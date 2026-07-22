using Microsoft.AspNetCore.Mvc;
using PRN232ASM.BuildingBlocks.Common.Models;
using PRN232ASM.PaperService.Application.DTOs.Requests;
using PRN232ASM.PaperService.Application.Interfaces;

namespace PRN232ASM.PaperService.Api.Controllers;

[ApiController]
[Route("api/bookmarks")]
public class BookmarksController : ControllerBase
{
    private readonly IPaperService _paperService;

    public BookmarksController(IPaperService paperService)
    {
        _paperService = paperService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<object>>> Create(
        [FromBody] BookmarkRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        var bookmark = await _paperService.AddBookmarkAsync(userId, request.PaperId, cancellationToken);
        return Ok(ApiResponse<object>.Ok(bookmark));
    }

    [HttpDelete("{paperId:guid}")]
    public async Task<ActionResult<ApiResponse>> Delete(Guid paperId, CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        await _paperService.RemoveBookmarkAsync(userId, paperId, cancellationToken);
        return Ok(ApiResponse.Ok("Bookmark removed."));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetByUser(
        [FromQuery] Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            return BadRequest(ApiResponse.Fail("userId is required."));
        }

        var bookmarks = await _paperService.GetBookmarksAsync(userId, cancellationToken);
        return Ok(ApiResponse<object>.Ok(bookmarks));
    }

    private Guid GetUserId()
    {
        if (Request.Headers.TryGetValue("X-User-Id", out var headerValue) &&
            Guid.TryParse(headerValue.FirstOrDefault(), out var userId))
        {
            return userId;
        }

        throw new UnauthorizedAccessException("X-User-Id header is required.");
    }
}
