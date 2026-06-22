using Microsoft.EntityFrameworkCore;
using PaperService.Application.Interfaces.Repositories;
using PaperService.Domain.Entities;

namespace PaperService.Infrastructure.Persistence.Repositories;

public class ResearchTopicRepository : IResearchTopicRepository
{
    private readonly PaperServiceDbContext _context;

    public ResearchTopicRepository(PaperServiceDbContext context) => _context = context;

    public async Task<IReadOnlyList<ResearchTopic>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.ResearchTopics
            .AsNoTracking()
            .Include(t => t.PaperTopics)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
}
