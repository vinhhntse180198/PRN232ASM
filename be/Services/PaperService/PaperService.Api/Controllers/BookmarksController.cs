using System.Security.Claims;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaperService.Application.DTOs.Responses;
using PaperService.Application.Interfaces;

namespace PaperService.Api.Controllers;

[ApiController]
[Route("api/bookmarks")]
[Authorize]
public class BookmarksController : ControllerBase
{
    private readonly IPaperService _paperService;

    public BookmarksController(IPaperService paperService) => _paperService = paperService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<BookmarkResponse>>>> GetMine(CancellationToken cancellationToken)
    {
        var userId = GetRequiredUserId();
        var result = await _paperService.GetBookmarksAsync(userId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<BookmarkResponse>>.Ok(result));
    }

    [HttpPost("{paperId:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Add(Guid paperId, CancellationToken cancellationToken)
    {
        await _paperService.AddBookmarkAsync(GetRequiredUserId(), paperId, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Bookmark added."));
    }

    [HttpDelete("{paperId:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> Remove(Guid paperId, CancellationToken cancellationToken)
    {
        await _paperService.RemoveBookmarkAsync(GetRequiredUserId(), paperId, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Bookmark removed."));
    }

    private Guid GetRequiredUserId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(sub, out var userId))
            throw new UnauthorizedAccessException("Invalid user token.");
        return userId;
    }
}
