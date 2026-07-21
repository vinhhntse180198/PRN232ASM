using PRN232ASM.PaperService.Domain.Entities;

namespace PRN232ASM.PaperService.Application.Interfaces.Repositories;

public interface IResearchTopicRepository
{
    Task<IReadOnlyList<ResearchTopic>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ResearchTopic?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddAsync(ResearchTopic topic, CancellationToken cancellationToken = default);
}
