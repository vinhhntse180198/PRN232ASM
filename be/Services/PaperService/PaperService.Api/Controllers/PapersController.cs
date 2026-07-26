using Microsoft.AspNetCore.Mvc;
using PRN232ASM.BuildingBlocks.Common.Models;
using PRN232ASM.PaperService.Application.DTOs.Requests;
using PRN232ASM.PaperService.Application.Interfaces;

namespace PRN232ASM.PaperService.Api.Controllers;

[ApiController]
[Route("api/papers")]
public class PapersController : ControllerBase
{
    private readonly IPaperService _paperService;

    public PapersController(IPaperService paperService)
    {
        _paperService = paperService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<object>>>> Search(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? keyword = null,
        [FromQuery] string? author = null,
        [FromQuery] string? journal = null,
        [FromQuery] Guid? topicId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _paperService.SearchAsync(new SearchPaperRequest
        {
            Page = page,
            PageSize = pageSize,
            Keyword = keyword,
            Author = author,
            Journal = journal,
            TopicId = topicId
        }, cancellationToken);

        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var paper = await _paperService.GetByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.Ok(paper));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<object>>> Create(
        [FromBody] CreatePaperRequest request,
        CancellationToken cancellationToken = default)
    {
        var paper = await _paperService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = paper.Id }, ApiResponse<object>.Ok(paper));
    }

    [HttpPost("import")]
    public async Task<ActionResult<ApiResponse<object>>> Import(
        [FromBody] CreatePaperRequest request,
        CancellationToken cancellationToken = default)
    {
        var paper = await _paperService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = paper.Id }, ApiResponse<object>.Ok(paper));
    }
}
