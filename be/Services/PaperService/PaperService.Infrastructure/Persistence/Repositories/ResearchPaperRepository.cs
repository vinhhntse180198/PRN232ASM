using Microsoft.EntityFrameworkCore;
using PaperService.Application.Interfaces.Repositories;
using PaperService.Domain.Entities;

namespace PaperService.Infrastructure.Persistence.Repositories;

public class ResearchPaperRepository : IResearchPaperRepository
{
    private readonly PaperServiceDbContext _context;

    public ResearchPaperRepository(PaperServiceDbContext context) => _context = context;

    public async Task<(IReadOnlyList<ResearchPaper> Items, int TotalCount)> SearchAsync(
        string? keyword,
        string? author,
        string? journal,
        Guid? topicId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ResearchPapers
            .AsNoTracking()
            .Include(p => p.Journal)
            .Include(p => p.PaperAuthors).ThenInclude(pa => pa.Author)
            .Include(p => p.PaperKeywords).ThenInclude(pk => pk.Keyword)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var term = keyword.Trim().ToLower();
            query = query.Where(p =>
                p.Title.ToLower().Contains(term) ||
                (p.Abstract != null && p.Abstract.ToLower().Contains(term)) ||
                p.PaperKeywords.Any(pk => pk.Keyword.Name.ToLower().Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(author))
        {
            var term = author.Trim().ToLower();
            query = query.Where(p => p.PaperAuthors.Any(pa => pa.Author.Name.ToLower().Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(journal))
        {
            var term = journal.Trim().ToLower();
            query = query.Where(p => p.Journal != null && p.Journal.Name.ToLower().Contains(term));
        }

        if (topicId.HasValue)
        {
            query = query.Where(p => p.PaperTopics.Any(pt => pt.TopicId == topicId.Value));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(p => p.PublishedYear)
            .ThenByDescending(p => p.CitationCount)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<ResearchPaper?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.ResearchPapers
            .Include(p => p.Journal)
            .Include(p => p.PaperAuthors).ThenInclude(pa => pa.Author)
            .Include(p => p.PaperKeywords).ThenInclude(pk => pk.Keyword)
            .Include(p => p.PaperTopics).ThenInclude(pt => pt.Topic)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
}
