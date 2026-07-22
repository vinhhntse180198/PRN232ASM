using Microsoft.AspNetCore.Mvc;
using PRN232ASM.BuildingBlocks.Common.Models;
using PRN232ASM.PaperService.Application.Interfaces;

namespace PRN232ASM.PaperService.Api.Controllers;

[ApiController]
[Route("api/keywords")]
public class KeywordsController : ControllerBase
{
    private readonly IPaperService _paperService;

    public KeywordsController(IPaperService paperService)
    {
        _paperService = paperService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetAll(CancellationToken cancellationToken = default)
    {
        var keywords = await _paperService.GetKeywordsAsync(cancellationToken);
        return Ok(ApiResponse<object>.Ok(keywords));
    }
}
