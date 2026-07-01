using PaperService.Application.DTOs;
using PaperService.Domain.Entities;

namespace PaperService.Application.Services;

internal static class PaperMapper
{
    public static PaperResponse Map(ResearchPaper paper, bool isBookmarked) => new()
    {
        Id = paper.Id,
        Title = paper.Title,
        Abstract = paper.Abstract,
        Doi = paper.Doi,
        JournalId = paper.JournalId,
        JournalName = paper.Journal?.Name,
        PublishedYear = paper.PublishedYear,
        PublishedDate = paper.PublishedDate?.ToDateTime(TimeOnly.MinValue),
        CitationCount = paper.CitationCount,
        IsOpenAccess = paper.IsOpenAccess,
        Url = paper.Url,
        ExternalId = paper.ExternalId,
        CreatedAt = paper.CreatedAt,
        Authors = paper.PaperAuthors
            .OrderBy(pa => pa.AuthorOrder ?? short.MaxValue)
            .Select(pa => new AuthorResponse
            {
                Id = pa.Author.Id,
                Name = pa.Author.Name,
                Affiliation = pa.Author.Affiliation,
                AuthorOrder = pa.AuthorOrder
            })
            .ToList(),
        Keywords = paper.PaperKeywords
            .Select(pk => pk.Keyword.Name)
            .OrderBy(k => k)
            .ToList(),
        IsBookmarked = isBookmarked
    };
}
