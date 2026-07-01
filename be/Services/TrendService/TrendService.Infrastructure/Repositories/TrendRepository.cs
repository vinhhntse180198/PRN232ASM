using TrendService.Application.Interfaces;
using TrendService.Domain.Entities;
using TrendService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace TrendService.Infrastructure.Repositories;

public class TrendRepository : ITrendRepository
{
    private readonly TrendDbContext _context;
    public TrendRepository(TrendDbContext context) => _context = context;

    public async Task<IReadOnlyList<PublicationTrend>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.PublicationTrends.AsNoTracking()
            .OrderByDescending(t => t.CalculatedAt)
            .ThenByDescending(t => t.Year)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<PublicationTrend>> GetKeywordTrendsAsync(short? year, CancellationToken cancellationToken = default)
    {
        var query = _context.PublicationTrends.AsNoTracking()
            .Where(t => t.KeywordName != null && t.KeywordName != "");

        if (year.HasValue)
            query = query.Where(t => t.Year == year.Value);

        return await query
            .OrderByDescending(t => t.PaperCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<PublicationTrend?> GetByTopicIdAsync(Guid topicId, CancellationToken cancellationToken = default)
        => await _context.PublicationTrends.AsNoTracking()
            .Where(t => t.TopicId == topicId)
            .OrderByDescending(t => t.Year)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<DateTime?> GetLatestCalculatedAtAsync(CancellationToken cancellationToken = default)
        => await _context.PublicationTrends.AsNoTracking()
            .MaxAsync(t => (DateTime?)t.CalculatedAt, cancellationToken);

    public async Task AddAsync(PublicationTrend trend, CancellationToken cancellationToken = default)
        => await _context.PublicationTrends.AddAsync(trend, cancellationToken);

    public async Task AddRangeAsync(IEnumerable<PublicationTrend> trends, CancellationToken cancellationToken = default)
        => await _context.PublicationTrends.AddRangeAsync(trends, cancellationToken);

    public async Task ClearAllAsync(CancellationToken cancellationToken = default)
        => await _context.PublicationTrends.ExecuteDeleteAsync(cancellationToken);
}
