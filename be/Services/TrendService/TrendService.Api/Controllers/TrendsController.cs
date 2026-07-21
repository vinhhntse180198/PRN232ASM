using Microsoft.AspNetCore.Mvc;
using PRN232ASM.BuildingBlocks.Common.Models;
using PRN232ASM.TrendService.Application.Interfaces;

namespace PRN232ASM.TrendService.Api.Controllers;

[ApiController]
[Route("api/trends")]
public class TrendsController : ControllerBase
{
    private readonly ITrendService _trendService;

    public TrendsController(ITrendService trendService)
    {
        _trendService = trendService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetTrends(
        [FromQuery] string? keyword = null,
        [FromQuery] int? year = null,
        CancellationToken cancellationToken = default)
    {
        var trends = await _trendService.GetTrendsAsync(keyword, year, cancellationToken);
        return Ok(ApiResponse<object>.Ok(trends));
    }
}

[ApiController]
[Route("api/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly ITrendService _trendService;

    public AnalyticsController(ITrendService trendService)
    {
        _trendService = trendService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetAnalytics(CancellationToken cancellationToken = default)
    {
        var analytics = await _trendService.GetAnalyticsAsync(cancellationToken);
        return Ok(ApiResponse<object>.Ok(analytics));
    }
}
