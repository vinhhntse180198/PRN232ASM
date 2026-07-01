using TrendService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace TrendService.Api.Controllers;

[ApiController]
[Route("api/trends")]
public class TrendsController : ControllerBase
{
    private readonly ITrendService _trendService;
    public TrendsController(ITrendService trendService) => _trendService = trendService;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(new { data = await _trendService.GetAllAsync(cancellationToken) });

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard([FromQuery] short? year, CancellationToken cancellationToken)
    {
        var token = Request.Headers.Authorization.FirstOrDefault()?.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
        var dashboard = await _trendService.GetDashboardAsync(year, token, cancellationToken);
        return Ok(new { data = dashboard });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        var result = await _trendService.RefreshFromLibraryAsync(cancellationToken);
        return Ok(new { data = result });
    }

    [HttpGet("topic/{topicId:guid}")]
    public async Task<IActionResult> GetByTopic(Guid topicId, CancellationToken cancellationToken)
    {
        var trend = await _trendService.GetByTopicIdAsync(topicId, cancellationToken);
        if (trend is null) return NotFound(new { message = "Trend not found." });
        return Ok(new { data = trend });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Application.DTOs.CreateTrendRequest request, CancellationToken cancellationToken)
    {
        try { return Ok(new { data = await _trendService.CreateAsync(request, cancellationToken) }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }
}
