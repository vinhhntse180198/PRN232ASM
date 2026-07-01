using SyncService.Application.DTOs;
using SyncService.Application.DTOs.OpenAlex;

namespace SyncService.Application.Services;

internal static class OpenAlexWorkMapper
{
    public static PaperImportRequest? ToImportRequest(OpenAlexWork work)
    {
        var title = work.Title ?? work.DisplayName;
        if (string.IsNullOrWhiteSpace(title)) return null;

        var authors = work.Authorships
            .Select(a => a.Author?.DisplayName)
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => n!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var keywords = work.Keywords
            .Select(k => k.DisplayName)
            .Concat(work.Topics.Select(t => t.DisplayName))
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => n!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(10)
            .ToList();

        DateOnly? publishedDate = null;
        if (!string.IsNullOrWhiteSpace(work.PublicationDate)
            && DateOnly.TryParse(work.PublicationDate, out var parsed))
            publishedDate = parsed;

        return new PaperImportRequest
        {
            Title = title.Trim(),
            Abstract = BuildAbstract(work),
            Doi = NormalizeDoi(work.Doi),
            ExternalId = ExtractOpenAlexId(work.Id),
            JournalName = work.PrimaryLocation?.Source?.DisplayName?.Trim(),
            PublishedYear = work.PublicationYear is > 0 and <= short.MaxValue
                ? (short)work.PublicationYear
                : null,
            PublishedDate = publishedDate,
            Url = ResolveReadUrl(work) ?? work.PrimaryLocation?.LandingPageUrl,
            CitationCount = work.CitedByCount,
            AuthorNames = authors.Count > 0 ? authors : null,
            Keywords = keywords.Count > 0 ? keywords : null
        };
    }

    public static OpenAlexPaperResult? ToSearchResult(OpenAlexWork work)
    {
        var title = work.Title ?? work.DisplayName;
        if (string.IsNullOrWhiteSpace(title)) return null;

        var openAlexId = ExtractOpenAlexId(work.Id);
        var authors = work.Authorships
            .Select(a => a.Author?.DisplayName)
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => n!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var keywords = work.Keywords
            .Select(k => k.DisplayName)
            .Concat(work.Topics.Select(t => t.DisplayName))
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => n!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(10)
            .ToList();

        var pdfUrl = work.PrimaryLocation?.PdfUrl?.Trim();
        var oaUrl = work.OpenAccess?.OaUrl?.Trim();
        var readUrl = ResolveReadUrl(work);
        var isOpenAccess = work.OpenAccess?.IsOa == true || work.PrimaryLocation?.IsOa == true;

        return new OpenAlexPaperResult
        {
            OpenAlexId = openAlexId,
            OpenAlexUrl = openAlexId is not null ? $"https://openalex.org/works/{openAlexId}" : null,
            Title = title.Trim(),
            Abstract = BuildAbstract(work),
            Doi = NormalizeDoi(work.Doi),
            JournalName = work.PrimaryLocation?.Source?.DisplayName?.Trim(),
            PublishedYear = work.PublicationYear is > 0 and <= short.MaxValue ? (short)work.PublicationYear : null,
            CitationCount = work.CitedByCount,
            Url = work.PrimaryLocation?.LandingPageUrl,
            PdfUrl = pdfUrl,
            OaUrl = oaUrl,
            IsOpenAccess = isOpenAccess,
            ReadUrl = readUrl,
            CanReadInApp = CanReadInApp(pdfUrl, oaUrl, readUrl, isOpenAccess),
            IsPaywalled = !isOpenAccess,
            HasPdf = !string.IsNullOrWhiteSpace(pdfUrl) || EndsWithPdf(oaUrl) || EndsWithPdf(readUrl),
            Authors = authors,
            Keywords = keywords
        };
    }

    public static bool CanReadInApp(string? pdfUrl, string? oaUrl, string? readUrl, bool isOpenAccess)
    {
        if (!string.IsNullOrWhiteSpace(pdfUrl)) return true;
        if (EndsWithPdf(oaUrl) || EndsWithPdf(readUrl)) return true;
        return isOpenAccess;
    }

    private static bool EndsWithPdf(string? url)
        => !string.IsNullOrWhiteSpace(url) && url.TrimEnd().EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);

    private static string? ResolveReadUrl(OpenAlexWork work)
    {
        var pdf = work.PrimaryLocation?.PdfUrl?.Trim();
        if (!string.IsNullOrWhiteSpace(pdf)) return pdf;

        var oa = work.OpenAccess?.OaUrl?.Trim();
        if (!string.IsNullOrWhiteSpace(oa)) return oa;

        return work.PrimaryLocation?.LandingPageUrl?.Trim();
    }

    private static string? BuildAbstract(OpenAlexWork work)
    {
        if (!string.IsNullOrWhiteSpace(work.Abstract))
            return work.Abstract.Trim();

        if (work.AbstractInvertedIndex is null || work.AbstractInvertedIndex.Count == 0)
            return null;

        var maxIndex = work.AbstractInvertedIndex.Values.SelectMany(v => v).DefaultIfEmpty(0).Max();
        var words = new string[maxIndex + 1];
        foreach (var (word, positions) in work.AbstractInvertedIndex)
        {
            foreach (var pos in positions)
                if (pos >= 0 && pos < words.Length)
                    words[pos] = word;
        }

        return string.Join(' ', words.Where(w => !string.IsNullOrEmpty(w)));
    }

    private static string? NormalizeDoi(string? doi)
    {
        if (string.IsNullOrWhiteSpace(doi)) return null;
        return doi
            .Replace("https://doi.org/", "", StringComparison.OrdinalIgnoreCase)
            .Replace("http://doi.org/", "", StringComparison.OrdinalIgnoreCase)
            .Trim();
    }

    private static string? ExtractOpenAlexId(string? id)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        var lastSlash = id.LastIndexOf('/');
        return lastSlash >= 0 ? id[(lastSlash + 1)..] : id;
    }
}
