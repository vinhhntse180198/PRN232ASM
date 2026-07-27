namespace PRN232ASM.TrendService.Application.Interfaces;

public interface IPaperCatalogClient
{
    Task<IReadOnlyList<PaperCatalogItem>> SearchPapersAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}

public sealed record PaperCatalogItem(int PublicationYear, IReadOnlyList<string> Keywords);
