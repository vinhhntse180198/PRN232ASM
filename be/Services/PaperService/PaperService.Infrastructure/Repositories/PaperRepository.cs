using PaperService.Application.Interfaces;
using PaperService.Domain.Entities;
using PaperService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace PaperService.Infrastructure.Repositories;

public class PaperRepository : IPaperRepository
{
    private readonly PaperDbContext _context;

    public PaperRepository(PaperDbContext context) => _context = context;

    private IQueryable<ResearchPaper> WithDetails()
        => _context.ResearchPapers
            .AsNoTracking()
            .Include(p => p.Journal)
            .Include(p => p.PaperAuthors).ThenInclude(pa => pa.Author)
            .Include(p => p.PaperKeywords).ThenInclude(pk => pk.Keyword);

    public async Task<(IReadOnlyList<ResearchPaper> Items, int Total)> SearchAsync(
        string? query,
        string? keyword,
        string? doi,
        short? year,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var q = WithDetails();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim().ToLowerInvariant();
            q = q.Where(p =>
                p.Title.ToLower().Contains(term) ||
                (p.Abstract != null && p.Abstract.ToLower().Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(doi))
        {
            var doiTerm = doi.Trim().ToLowerInvariant();
            q = q.Where(p => p.Doi != null && p.Doi.ToLower().Contains(doiTerm));
        }

        if (year.HasValue)
            q = q.Where(p => p.PublishedYear == year.Value);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.Trim().ToLowerInvariant();
            q = q.Where(p => p.PaperKeywords.Any(pk => pk.Keyword.Name.Contains(kw)));
        }

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderByDescending(p => p.PublishedYear)
            .ThenByDescending(p => p.CitationCount)
            .ThenByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<ResearchPaper?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        => await WithDetails().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<bool> ExistsByDoiAsync(string doi, CancellationToken cancellationToken = default)
        => await _context.ResearchPapers.AnyAsync(
            p => p.Doi != null && p.Doi.ToLower() == doi.ToLower(),
            cancellationToken);

    public async Task AddAsync(ResearchPaper paper, CancellationToken cancellationToken = default)
        => await _context.ResearchPapers.AddAsync(paper, cancellationToken);
}
