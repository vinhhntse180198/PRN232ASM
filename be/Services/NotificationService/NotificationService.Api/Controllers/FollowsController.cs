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

    [HttpGet]
    public async Task<ActionResult<ApiResponse<FollowsSummaryDto>>> GetFollows(
        [FromQuery] Guid userId,
        CancellationToken cancellationToken)
    {
        var data = await _followService.GetFollowsAsync(userId, cancellationToken);
        return Ok(ApiResponse<FollowsSummaryDto>.Ok(data));
    }

    [HttpPost("topic")]
    public async Task<ActionResult<ApiResponse<object>>> FollowTopic(
        [FromBody] FollowTopicRequest request,
        CancellationToken cancellationToken)
    {
        await _followService.FollowTopicAsync(request, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Topic followed."));
    }

    [HttpPost("keyword")]
    public async Task<ActionResult<ApiResponse<object>>> FollowKeyword(
        [FromBody] FollowKeywordRequest request,
        CancellationToken cancellationToken)
    {
        await _followService.FollowKeywordAsync(request, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Keyword followed."));
    }

    [HttpPost("journal")]
    public async Task<ActionResult<ApiResponse<object>>> FollowJournal(
        [FromBody] FollowJournalRequest request,
        CancellationToken cancellationToken)
    {
        await _followService.FollowJournalAsync(request, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Journal followed."));
    }

    [HttpDelete("topic/{topicId:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> UnfollowTopic(
        Guid topicId,
        [FromQuery] Guid userId,
        CancellationToken cancellationToken)
    {
        await _followService.UnfollowTopicAsync(userId, topicId, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Topic unfollowed."));
    }

    [HttpDelete("keyword/{keywordId:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> UnfollowKeyword(
        Guid keywordId,
        [FromQuery] Guid userId,
        CancellationToken cancellationToken)
    {
        await _followService.UnfollowKeywordAsync(userId, keywordId, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Keyword unfollowed."));
    }

    [HttpDelete("journal/{journalId:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> UnfollowJournal(
        Guid journalId,
        [FromQuery] Guid userId,
        CancellationToken cancellationToken)
    {
        await _followService.UnfollowJournalAsync(userId, journalId, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Journal unfollowed."));
    }
}
