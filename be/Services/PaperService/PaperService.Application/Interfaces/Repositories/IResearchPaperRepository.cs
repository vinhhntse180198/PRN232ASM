using PaperService.Domain.Entities;

namespace PaperService.Application.Interfaces.Repositories;

public interface IResearchPaperRepository
{
    Task<(IReadOnlyList<ResearchPaper> Items, int TotalCount)> SearchAsync(
        string? keyword,
        string? author,
        string? journal,
        Guid? topicId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ResearchPaper?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}
