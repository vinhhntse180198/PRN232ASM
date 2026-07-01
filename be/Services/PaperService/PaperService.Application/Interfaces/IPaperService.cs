using PaperService.Application.DTOs;

namespace PaperService.Application.Interfaces;

public interface IPaperService
{
    Task<PagedResult<PaperResponse>> SearchAsync(SearchPaperRequest request, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<PaperResponse?> GetByIdAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<PaperResponse> CreateAsync(CreatePaperRequest request, CancellationToken cancellationToken = default);
}
