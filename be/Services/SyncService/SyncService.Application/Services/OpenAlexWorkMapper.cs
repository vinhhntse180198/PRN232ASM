using SyncService.Application.DTOs;
using SyncService.Application.DTOs.OpenAlex;

namespace SyncService.Application.Services;

public static class OpenAlexWorkMapper
{
    public static PaperImportRequest? ToImportRequest(OpenAlexWork work)
    {
        var title = work.Title ?? work.DisplayName;
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(work.Id))
            return null;

        var url = GetSourceUrl(work);

        return new PaperImportRequest(
            Title: title,
            Abstract: ReconstructAbstract(work.AbstractInvertedIndex),
            PublicationYear: work.PublicationYear ?? 0,
            Doi: NormalizeDoi(work.Doi),
            CitationCount: work.CitedByCount,
            JournalName: work.PrimaryLocation?.Source?.DisplayName ?? "Unknown",
            Authors: work.Authorships
                .Select(a => a.Author?.DisplayName)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => n!)
                .Distinct()
                .ToList(),
            Keywords: work.Keywords
                .Select(k => k.DisplayName)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => n!)
                .Distinct()
                .ToList(),
            Topics: work.Topics
                .Select(t => t.DisplayName)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => n!)
                .Distinct()
                .ToList(),
            Url: url,
            PdfUrl: GetPdfUrl(work));
    }

    private static string? GetSourceUrl(OpenAlexWork work)
    {
        var primary = work.PrimaryLocation?.LandingPageUrl;
        if (IsRealUrl(primary))
            return primary;

        var relatedUrl = work.RelatedUrls?
            .FirstOrDefault(u => u.RelationshipType == "publisher" || u.RelationshipType == "host_venue")
            ?.Url;
        if (IsRealUrl(relatedUrl))
            return relatedUrl;

        return GetOpenAlexUrl(work);
    }

    private static string? GetPdfUrl(OpenAlexWork work)
    {
        var oaLocation = work.BestOaLocation;
        if (oaLocation is null) return null;

        if (IsRealUrl(oaLocation.PdfUrl))
            return oaLocation.PdfUrl;

        if (IsRealUrl(oaLocation.LandingPageUrl))
            return oaLocation.LandingPageUrl;

        return null;
    }

    private static bool IsRealUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;
        var trimmed = url.Trim().TrimEnd(',', ';', '.', '>');
        if (trimmed.StartsWith("https://web.archive.org/", StringComparison.OrdinalIgnoreCase)) return false;
        return Uri.TryCreate(trimmed, UriKind.Absolute, out var uri)
               && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }

    private static string? GetOpenAlexUrl(OpenAlexWork work)
    {
        if (string.IsNullOrWhiteSpace(work.Id)) return null;
        return work.Id.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            ? work.Id
            : $"https://openalex.org/{work.Id}";
    }

    private static string? ResolveDoiUrl(string? doi)
    {
        if (string.IsNullOrWhiteSpace(doi)) return null;
        return $"https://doi.org/{NormalizeDoi(doi)}";
    }

    private static string? ReconstructAbstract(Dictionary<string, List<int>>? invertedIndex)
    {
        if (invertedIndex is null || invertedIndex.Count == 0)
            return null;

        var maxIndex = invertedIndex.Values.SelectMany(v => v).DefaultIfEmpty(-1).Max();
        if (maxIndex < 0) return null;

        var words = new string[maxIndex + 1];
        foreach (var (word, positions) in invertedIndex)
        {
            foreach (var pos in positions)
            {
                if (pos >= 0 && pos < words.Length)
                    words[pos] = word;
            }
        }

        return string.Join(' ', words.Where(w => !string.IsNullOrWhiteSpace(w)));
    }

    private static string? NormalizeDoi(string? doi)
    {
        if (string.IsNullOrWhiteSpace(doi)) return null;
        return doi.Replace("https://doi.org/", "", StringComparison.OrdinalIgnoreCase);
    }
}
