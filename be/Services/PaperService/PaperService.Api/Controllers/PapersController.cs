using PaperService.Api.Extensions;
using PaperService.Application.DTOs;
using PaperService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PaperService.Api.Controllers;

[ApiController]
[Route("api/papers")]
public class PapersController : ControllerBase
{
    private readonly IPaperService _paperService;

    public PapersController(IPaperService paperService) => _paperService = paperService;

    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string? query,
        [FromQuery] string? keyword,
        [FromQuery] string? doi,
        [FromQuery] short? year,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var request = new SearchPaperRequest
        {
            Query = query,
            Keyword = keyword,
            Doi = doi,
            Year = year,
            Page = page,
            PageSize = pageSize
        };

        var userId = User.GetUserId();
        var result = await _paperService.SearchAsync(request, userId, cancellationToken);
        return Ok(new { data = result });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var paper = await _paperService.GetByIdAsync(id, userId, cancellationToken);
        if (paper is null) return NotFound(new { message = "Paper not found." });
        return Ok(new { data = paper });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaperRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var paper = await _paperService.CreateAsync(request, cancellationToken);
            return Ok(new { data = paper });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
