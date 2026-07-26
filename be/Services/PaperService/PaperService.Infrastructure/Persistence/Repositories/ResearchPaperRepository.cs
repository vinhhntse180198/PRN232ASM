using Microsoft.EntityFrameworkCore;
using PRN232ASM.BuildingBlocks.Common.Models;
using PRN232ASM.PaperService.Application.Interfaces.Repositories;
using PRN232ASM.PaperService.Domain.Entities;

namespace PRN232ASM.PaperService.Infrastructure.Persistence.Repositories;

public class ResearchPaperRepository : IResearchPaperRepository
{
    private readonly PaperServiceDbContext _context;

    public ResearchPaperRepository(PaperServiceDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ResearchPaper paper, CancellationToken cancellationToken = default)
    {
        await _context.ResearchPapers.AddAsync(paper, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ResearchPapers.CountAsync(cancellationToken);
    }

    public async Task<ResearchPaper?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ResearchPapers
            .Include(p => p.Journal)
            .Include(p => p.PaperAuthors).ThenInclude(pa => pa.Author)
            .Include(p => p.PaperKeywords).ThenInclude(pk => pk.Keyword)
            .Include(p => p.PaperTopics).ThenInclude(pt => pt.Topic)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<ResearchPaper>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ResearchPapers
            .Include(p => p.Journal)
            .Include(p => p.PaperAuthors).ThenInclude(pa => pa.Author)
            .Include(p => p.PaperKeywords).ThenInclude(pk => pk.Keyword)
            .Include(p => p.PaperTopics).ThenInclude(pt => pt.Topic)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<ResearchPaper>> SearchAsync(
        int page,
        int pageSize,
        string? keyword,
        string? author,
        string? journal,
        Guid? topicId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ResearchPapers
            .Include(p => p.Journal)
            .Include(p => p.PaperAuthors).ThenInclude(pa => pa.Author)
            .Include(p => p.PaperKeywords).ThenInclude(pk => pk.Keyword)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var term = keyword.Trim().ToLower();
            query = query.Where(p =>
                p.Title.ToLower().Contains(term) ||
                p.Abstract.ToLower().Contains(term) ||
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
            query = query.Where(p => p.Journal.Name.ToLower().Contains(term));
        }

        if (topicId.HasValue)
        {
            query = query.Where(p => p.PaperTopics.Any(pt => pt.TopicId == topicId.Value));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(p => p.PublicationYear)
            .ThenByDescending(p => p.CitationCount)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<ResearchPaper>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
