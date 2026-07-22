using PRN232ASM.BuildingBlocks.Common.Models;
using PRN232ASM.PaperService.Domain.Entities;

namespace PRN232ASM.PaperService.Application.Interfaces.Repositories;

public interface IResearchPaperRepository
{
    Task<PagedResult<ResearchPaper>> SearchAsync(
        int page,
        int pageSize,
        string? keyword,
        string? author,
        string? journal,
        CancellationToken cancellationToken = default);

    Task<ResearchPaper?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByDoiOrTitleAsync(string? doi, string title, CancellationToken cancellationToken = default);
    Task AddAsync(ResearchPaper paper, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ResearchPaper>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
}
