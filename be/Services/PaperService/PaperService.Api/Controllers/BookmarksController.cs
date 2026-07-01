using PaperService.Api.Extensions;
using PaperService.Application.DTOs;
using PaperService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PaperService.Api.Controllers;

[ApiController]
[Route("api/bookmarks")]
[Authorize]
public class BookmarksController : ControllerBase
{
    private readonly IBookmarkService _bookmarkService;

    public BookmarksController(IBookmarkService bookmarkService) => _bookmarkService = bookmarkService;

    [HttpGet]
    public async Task<IActionResult> GetMyBookmarks(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized(new { message = "Invalid token." });

        var bookmarks = await _bookmarkService.GetMyBookmarksAsync(userId.Value, cancellationToken);
        return Ok(new { data = bookmarks });
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateBookmarkRequest request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized(new { message = "Invalid token." });

        try
        {
            var bookmark = await _bookmarkService.AddAsync(userId.Value, request.PaperId, cancellationToken);
            return Ok(new { data = bookmark });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{paperId:guid}")]
    public async Task<IActionResult> Remove(Guid paperId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized(new { message = "Invalid token." });

        try
        {
            await _bookmarkService.RemoveAsync(userId.Value, paperId, cancellationToken);
            return Ok(new { message = "Bookmark removed." });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
