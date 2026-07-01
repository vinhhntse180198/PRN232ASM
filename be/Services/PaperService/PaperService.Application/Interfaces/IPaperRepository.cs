using PaperService.Application.DTOs;
using PaperService.Domain.Entities;

namespace PaperService.Application.Interfaces;

public interface IPaperRepository
{
    Task<(IReadOnlyList<ResearchPaper> Items, int Total)> SearchAsync(
        string? query,
        string? keyword,
        string? doi,
        short? year,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<ResearchPaper?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByDoiAsync(string doi, CancellationToken cancellationToken = default);
    Task AddAsync(ResearchPaper paper, CancellationToken cancellationToken = default);
}
