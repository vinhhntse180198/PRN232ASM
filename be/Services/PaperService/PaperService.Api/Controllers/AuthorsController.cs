using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaperService.Application.DTOs.Responses;
using PaperService.Application.Interfaces;

namespace PaperService.Api.Controllers;

[ApiController]
[Route("api/authors")]
public class AuthorsController : ControllerBase
{
    private readonly IPaperService _paperService;

    public AuthorsController(IPaperService paperService) => _paperService = paperService;

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<AuthorSummaryResponse>>>> Search(
        [FromQuery] string q,
        CancellationToken cancellationToken)
    {
        var result = await _paperService.SearchAuthorsAsync(q, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<AuthorSummaryResponse>>.Ok(result));
    }
}
