using PaperService.Domain.Entities;

namespace PaperService.Application.Interfaces.Repositories;

public interface IResearchTopicRepository
{
    Task<IReadOnlyList<ResearchTopic>> GetAllAsync(CancellationToken cancellationToken = default);
}
