using Microsoft.EntityFrameworkCore;
using PaperService.Application.DTOs;
using PaperService.Application.Interfaces;
using PaperService.Infrastructure.Data;

namespace PaperService.Infrastructure.Repositories;

public class AnalyticsRepository : IAnalyticsRepository
{
    private readonly PaperDbContext _db;

    public AnalyticsRepository(PaperDbContext db) => _db = db;

    public async Task<LibraryAnalyticsDto> GetLibraryAnalyticsAsync(short? year, CancellationToken cancellationToken = default)
    {
        var papers = _db.ResearchPapers.AsNoTracking().AsQueryable();
        if (year.HasValue)
            papers = papers.Where(p => p.PublishedYear == year.Value);

        var total = await papers.CountAsync(cancellationToken);

        var papersByYear = await papers
            .Where(p => p.PublishedYear != null)
            .GroupBy(p => p.PublishedYear!.Value)
            .Select(g => new YearCountDto { Year = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Year)
            .ToListAsync(cancellationToken);

        var openAccessByYear = await papers
            .Where(p => p.PublishedYear != null)
            .GroupBy(p => p.PublishedYear!.Value)
            .Select(g => new OpenAccessYearDto
            {
                Year = g.Key,
                TotalPapers = g.Count(),
                OpenAccessPapers = g.Count(p => p.IsOpenAccess),
                OpenAccessPercent = g.Count() == 0 ? 0 : Math.Round((decimal)g.Count(p => p.IsOpenAccess) * 100m / g.Count(), 1)
            })
            .OrderByDescending(x => x.Year)
            .ToListAsync(cancellationToken);

        var keywordYearStats = await (
            from pk in _db.PaperKeywords.AsNoTracking()
            join p in papers on pk.PaperId equals p.Id
            join k in _db.Keywords.AsNoTracking() on pk.KeywordId equals k.Id
            where p.PublishedYear != null
            group new { p, k } by new { k.Name, Year = p.PublishedYear!.Value } into g
            select new KeywordYearStatDto
            {
                Keyword = g.Key.Name,
                Year = g.Key.Year,
                PaperCount = g.Count(),
                CitationSum = g.Sum(x => x.p.CitationCount)
            })
            .OrderByDescending(x => x.PaperCount)
            .ToListAsync(cancellationToken);

        return new LibraryAnalyticsDto
        {
            TotalPapers = total,
            PapersByYear = papersByYear,
            KeywordYearStats = keywordYearStats,
            OpenAccessByYear = openAccessByYear
        };
    }

    public async Task<IReadOnlyList<BookmarkKeywordDto>> GetBookmarkKeywordStatsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await (
            from b in _db.Bookmarks.AsNoTracking()
            where b.UserId == userId
            from pk in b.Paper.PaperKeywords
            let k = pk.Keyword
            group b by k.Name into g
            select new BookmarkKeywordDto
            {
                Keyword = g.Key,
                PaperCount = g.Count(),
                CitationSum = g.Sum(x => x.Paper.CitationCount)
            })
            .OrderByDescending(x => x.PaperCount)
            .Take(20)
            .ToListAsync(cancellationToken);
    }
}
