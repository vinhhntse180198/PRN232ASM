using Microsoft.Extensions.Logging;
using PRN232ASM.BuildingBlocks.GrpcContracts.Papers;
using PRN232ASM.TrendService.Application.Interfaces;

namespace PRN232ASM.TrendService.Infrastructure.Grpc;

public sealed class PaperCatalogGrpcClient : IPaperCatalogClient
{
    private readonly PaperCatalog.PaperCatalogClient _client;
    private readonly ILogger<PaperCatalogGrpcClient> _logger;

    public PaperCatalogGrpcClient(
        PaperCatalog.PaperCatalogClient client,
        ILogger<PaperCatalogGrpcClient> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<IReadOnlyList<PaperCatalogItem>> SearchPapersAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("gRPC SearchPapers page={Page} pageSize={PageSize}", page, pageSize);

        var response = await _client.SearchPapersAsync(
            new SearchPapersRequest
            {
                Page = page,
                PageSize = pageSize
            },
            cancellationToken: cancellationToken);

        return response.Items
            .Select(item => new PaperCatalogItem(
                item.PublicationYear,
                item.Keywords.ToList()))
            .ToList();
    }
}
