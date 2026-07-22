using Microsoft.EntityFrameworkCore;
using PRN232ASM.PaperService.Application.Interfaces.Repositories;
using PRN232ASM.PaperService.Domain.Entities;

namespace PRN232ASM.PaperService.Infrastructure.Persistence.Repositories;

public class ResearchTopicRepository : IResearchTopicRepository
{
    private readonly PaperServiceDbContext _context;

    public ResearchTopicRepository(PaperServiceDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ResearchTopic topic, CancellationToken cancellationToken = default)
    {
        await _context.ResearchTopics.AddAsync(topic, cancellationToken);
    }

    public async Task<IReadOnlyList<ResearchTopic>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ResearchTopics.AsNoTracking().OrderBy(t => t.Name).ToListAsync(cancellationToken);
    }

    public async Task<ResearchTopic?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.ResearchTopics.FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
    }
}
