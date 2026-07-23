using Microsoft.AspNetCore.Mvc;
using PRN232ASM.BuildingBlocks.Common.Models;
using PRN232ASM.PaperService.Application.DTOs.Requests;
using PRN232ASM.PaperService.Application.Interfaces;

namespace PRN232ASM.PaperService.Api.Controllers;

[ApiController]
[Route("api/papers")]
public class PapersController : ControllerBase
{
    private readonly IPaperService _paperService;

    public PapersController(IPaperService paperService)
    {
        _paperService = paperService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<object>>>> Search(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? keyword = null,
        [FromQuery] string? author = null,
        [FromQuery] string? journal = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _paperService.SearchAsync(new SearchPaperRequest
        {
            Page = page,
            PageSize = pageSize,
            Keyword = keyword,
            Author = author,
            Journal = journal
        }, cancellationToken);

        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var paper = await _paperService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.Ok(paper));
    }

    /// <summary>
    /// REST entry point that calls RecommendationService over gRPC.
    /// </summary>
    [HttpGet("{id:guid}/recommendations")]
    public async Task<ActionResult<ApiResponse<object>>> GetRecommendations(
        Guid id,
        [FromQuery] int limit = 5,
        CancellationToken cancellationToken = default)
    {
        var items = await _paperService.GetRecommendationsAsync(id, limit, cancellationToken);
        return Ok(ApiResponse<object>.Ok(items));
    }

    /// <summary>
    /// REST → PricingService gRPC (impact / "pricing" score from Paper DB fields).
    /// </summary>
    [HttpGet("{id:guid}/impact-score")]
    public async Task<ActionResult<ApiResponse<object>>> GetImpactScore(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _paperService.GetImpactScoreAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.Ok(result));
    }

    /// <summary>
    /// REST → InferenceService gRPC (AI/ML-style insights from title/abstract in Paper DB).
    /// </summary>
    [HttpGet("{id:guid}/insights")]
    public async Task<ActionResult<ApiResponse<object>>> GetInsights(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _paperService.GetInsightsAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<object>>> Create(
        [FromBody] CreatePaperRequest request,
        CancellationToken cancellationToken = default)
    {
        var paper = await _paperService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = paper.Id }, ApiResponse<object>.Ok(paper));
    }

    [HttpPost("import")]
    public async Task<ActionResult<ApiResponse<object>>> Import(
        [FromBody] ImportPaperRequest request,
        CancellationToken cancellationToken = default)
    {
        var created = await _paperService.ImportAsync(request, cancellationToken);
        if (!created)
            return Conflict(ApiResponse.Fail("Paper already exists."));

        return Ok(ApiResponse<object>.Ok(new { imported = true }));
    }
}
