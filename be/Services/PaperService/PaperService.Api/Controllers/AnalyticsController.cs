using PaperService.Api.Extensions;
using PaperService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PaperService.Api.Controllers;

[ApiController]
[Route("api/papers")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analytics;

    public AnalyticsController(IAnalyticsService analytics) => _analytics = analytics;

    [HttpGet("analytics")]
    public async Task<IActionResult> GetLibraryAnalytics([FromQuery] short? year, CancellationToken cancellationToken)
    {
        var data = await _analytics.GetLibraryAnalyticsAsync(year, cancellationToken);
        return Ok(new { data });
    }
}

[ApiController]
[Route("api/bookmarks")]
public class BookmarkAnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analytics;

    public BookmarkAnalyticsController(IAnalyticsService analytics) => _analytics = analytics;

    [HttpGet("analytics")]
    [Authorize]
    public async Task<IActionResult> GetBookmarkAnalytics(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized(new { message = "Invalid token." });

        var data = await _analytics.GetBookmarkKeywordStatsAsync(userId.Value, cancellationToken);
        return Ok(new { data });
    }
}
