using Microsoft.AspNetCore.Mvc;
using PRN232ASM.BuildingBlocks.Common.Models;
using PRN232ASM.PaperService.Application.Interfaces;

namespace PRN232ASM.PaperService.Api.Controllers;

[ApiController]
[Route("api/journals")]
public class JournalsController : ControllerBase
{
    private readonly IPaperService _paperService;

    public JournalsController(IPaperService paperService)
    {
        _paperService = paperService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetAll(CancellationToken cancellationToken = default)
    {
        var journals = await _paperService.GetJournalsAsync(cancellationToken);
        return Ok(ApiResponse<object>.Ok(journals));
    }
}
