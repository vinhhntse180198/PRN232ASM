using Microsoft.AspNetCore.Mvc;
using PRN232ASM.BuildingBlocks.Common.Models;
using PRN232ASM.PaperService.Application.Interfaces;

namespace PRN232ASM.PaperService.Api.Controllers;

[ApiController]
[Route("api/authors")]
public class AuthorsController : ControllerBase
{
    private readonly IPaperService _paperService;

    public AuthorsController(IPaperService paperService)
    {
        _paperService = paperService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetAll(CancellationToken cancellationToken = default)
    {
        var authors = await _paperService.GetAuthorsAsync(cancellationToken);
        return Ok(ApiResponse<object>.Ok(authors));
    }
}
