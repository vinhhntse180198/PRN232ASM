using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using PRN232ASM.BuildingBlocks.Common.Exceptions;
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

    [HttpDelete("topic/{topicId:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> UnfollowTopic(
        Guid topicId,
        [FromQuery] Guid userId,
        CancellationToken cancellationToken)
    {
        await _followService.UnfollowTopicAsync(userId, topicId, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Topic unfollowed."));
    }
}
