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

    /// <summary>
    /// REST → InventoryService gRPC (journal yearly capacity from Paper DB counts).
    /// </summary>
    [HttpGet("{id:guid}/capacity")]
    public async Task<ActionResult<ApiResponse<object>>> GetCapacity(
        Guid id,
        [FromQuery] int? year = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _paperService.GetJournalCapacityAsync(id, year, cancellationToken);
        return Ok(ApiResponse<object>.Ok(result));
    }
}
