using Microsoft.AspNetCore.Mvc;
using PRN232ASM.BuildingBlocks.Common.Models;
using PRN232ASM.PaperService.Application.Interfaces;

namespace PRN232ASM.PaperService.Api.Controllers;

[ApiController]
[Route("api/topics")]
public class TopicsController : ControllerBase
{
    private readonly IPaperService _paperService;

    public TopicsController(IPaperService paperService)
    {
        _paperService = paperService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetAll(CancellationToken cancellationToken = default)
    {
        var topics = await _paperService.GetTopicsAsync(cancellationToken);
        return Ok(ApiResponse<object>.Ok(topics));
    }
}
