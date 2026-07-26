using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using PRN232ASM.BuildingBlocks.Common.Models;

namespace NotificationService.Api.Controllers;

[ApiController]
[Route("api/follows")]
public class FollowsController : ControllerBase
{
    private readonly IFollowService _followService;

    public FollowsController(IFollowService followService)
    {
        _followService = followService;
    }

    private Guid GetUserId()
    {
        if (Request.Headers.TryGetValue("X-User-Id", out var headerValue) &&
            Guid.TryParse(headerValue.FirstOrDefault(), out var userId) &&
            userId != Guid.Empty)
        {
            return userId;
        }
        throw new UnauthorizedAccessException("X-User-Id header is required.");
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<FollowsSummaryDto>>> GetFollows(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var data = await _followService.GetFollowsAsync(userId, cancellationToken);
        return Ok(ApiResponse<FollowsSummaryDto>.Ok(data));
    }

    [HttpPost("topic")]
    public async Task<ActionResult<ApiResponse<object>>> FollowTopic(
        [FromBody] FollowTopicRequest request,
        CancellationToken cancellationToken)
    {
        request = request with { UserId = GetUserId() };
        await _followService.FollowTopicAsync(request, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Topic followed."));
    }

    [HttpDelete("topic/{topicId:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> UnfollowTopic(
        Guid topicId,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        await _followService.UnfollowTopicAsync(userId, topicId, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Topic unfollowed."));
    }

    [HttpPost("keyword")]
    public async Task<ActionResult<ApiResponse<object>>> FollowKeyword(
        [FromBody] FollowKeywordRequest request,
        CancellationToken cancellationToken)
    {
        request = request with { UserId = GetUserId() };
        await _followService.FollowKeywordAsync(request, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Keyword followed."));
    }

    [HttpDelete("keyword/{keywordId:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> UnfollowKeyword(
        Guid keywordId,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        await _followService.UnfollowKeywordAsync(userId, keywordId, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Keyword unfollowed."));
    }

    [HttpPost("journal")]
    public async Task<ActionResult<ApiResponse<object>>> FollowJournal(
        [FromBody] FollowJournalRequest request,
        CancellationToken cancellationToken)
    {
        request = request with { UserId = GetUserId() };
        await _followService.FollowJournalAsync(request, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Journal followed."));
    }

    [HttpDelete("journal/{journalId:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> UnfollowJournal(
        Guid journalId,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        await _followService.UnfollowJournalAsync(userId, journalId, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Journal unfollowed."));
    }
}
