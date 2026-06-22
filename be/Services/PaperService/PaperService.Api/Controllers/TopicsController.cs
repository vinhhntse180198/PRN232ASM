using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaperService.Application.DTOs.Responses;
using PaperService.Application.Interfaces;

namespace PaperService.Api.Controllers;

[ApiController]
[Route("api/topics")]
public class TopicsController : ControllerBase
{
    private readonly IPaperService _paperService;

    public TopicsController(IPaperService paperService) => _paperService = paperService;

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<TopicListItemResponse>>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _paperService.GetTopicsAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<TopicListItemResponse>>.Ok(result));
    }
}
