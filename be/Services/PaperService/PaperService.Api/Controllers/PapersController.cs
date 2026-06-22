using System.Security.Claims;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaperService.Application.DTOs.Requests;
using PaperService.Application.DTOs.Responses;
using PaperService.Application.Interfaces;

namespace PaperService.Api.Controllers;

[ApiController]
[Route("api/papers")]
public class PapersController : ControllerBase
{
    private readonly IPaperService _paperService;

    public PapersController(IPaperService paperService) => _paperService = paperService;

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<PagedResult<PaperListItemResponse>>>> Search(
        [FromQuery] SearchPaperRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _paperService.SearchAsync(request, GetUserId(), cancellationToken);
        return Ok(ApiResponse<PagedResult<PaperListItemResponse>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<PaperDetailResponse>>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _paperService.GetDetailAsync(id, GetUserId(), cancellationToken);
        return Ok(ApiResponse<PaperDetailResponse>.Ok(result));
    }

    private Guid? GetUserId()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(sub, out var userId) ? userId : null;
    }
}
