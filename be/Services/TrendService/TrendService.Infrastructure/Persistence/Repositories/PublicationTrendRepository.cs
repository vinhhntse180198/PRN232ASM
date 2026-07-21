using Microsoft.EntityFrameworkCore;
using PRN232ASM.TrendService.Application.Interfaces.Repositories;
using PRN232ASM.TrendService.Domain.Entities;

namespace PRN232ASM.TrendService.Infrastructure.Persistence.Repositories;

public class PublicationTrendRepository : IPublicationTrendRepository
{
    private readonly TrendServiceDbContext _context;

    public PublicationTrendRepository(TrendServiceDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PublicationTrend trend, CancellationToken cancellationToken = default)
    {
        await _context.PublicationTrends.AddAsync(trend, cancellationToken);
    }

    public async Task<IReadOnlyList<PublicationTrend>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PublicationTrends.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PublicationTrend>> GetAsync(string? keyword, int? year, CancellationToken cancellationToken = default)
    {
        var query = _context.PublicationTrends.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var term = keyword.Trim().ToLower();
            query = query.Where(t => t.Keyword.ToLower().Contains(term));
        }

        if (year.HasValue)
        {
            query = query.Where(t => t.Year == year.Value);
        }

        return await query.OrderByDescending(t => t.PaperCount).ThenBy(t => t.Keyword).ToListAsync(cancellationToken);
    }

    public async Task<PublicationTrend?> GetByKeywordYearAsync(string keyword, int year, Guid? topicId, CancellationToken cancellationToken = default)
    {
        return await _context.PublicationTrends
            .FirstOrDefaultAsync(t => t.Keyword == keyword && t.Year == year && t.TopicId == topicId, cancellationToken);
    }

    public async Task RemoveAllAsync(CancellationToken cancellationToken = default)
    {
        await _context.PublicationTrends.ExecuteDeleteAsync(cancellationToken);
    }
}
