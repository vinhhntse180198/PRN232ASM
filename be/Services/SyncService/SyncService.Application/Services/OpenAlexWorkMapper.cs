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

        return new PaperImportRequest(
            ExternalId: work.Id,
            Title: title,
            Abstract: ReconstructAbstract(work.AbstractInvertedIndex),
            PublicationYear: work.PublicationYear,
            Doi: NormalizeDoi(work.Doi),
            CitationCount: work.CitedByCount,
            JournalName: work.PrimaryLocation?.Source?.DisplayName,
            AuthorNames: work.Authorships
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
                .ToList());
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
