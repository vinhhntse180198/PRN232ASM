using Grpc.Core;
using PRN232ASM.BuildingBlocks.GrpcContracts.Papers;
using PRN232ASM.PaperService.Application.DTOs.Requests;
using PRN232ASM.PaperService.Application.Interfaces;

namespace PRN232ASM.PaperService.Api.Grpc;

public sealed class PaperCatalogGrpcService : PaperCatalog.PaperCatalogBase
{
    private readonly IPaperService _paperService;

    public PaperCatalogGrpcService(IPaperService paperService)
    {
        _paperService = paperService;
    }

    public override async Task<SearchPapersResponse> SearchPapers(
        SearchPapersRequest request,
        ServerCallContext context)
    {
        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 20 : Math.Min(request.PageSize, 100);

        var result = await _paperService.SearchAsync(new SearchPaperRequest
        {
            Page = page,
            PageSize = pageSize,
            Keyword = string.IsNullOrWhiteSpace(request.Keyword) ? null : request.Keyword
        }, context.CancellationToken);

        var response = new SearchPapersResponse
        {
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };

        foreach (var paper in result.Items)
        {
            var item = new PaperSummary
            {
                Id = paper.Id.ToString(),
                PublicationYear = paper.PublicationYear
            };
            item.Keywords.AddRange(paper.Keywords);
            response.Items.Add(item);
        }

        return response;
    }
}
