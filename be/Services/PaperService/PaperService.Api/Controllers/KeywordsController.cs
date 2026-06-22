using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaperService.Application.DTOs.Responses;
using PaperService.Application.Interfaces;

namespace PaperService.Api.Controllers;

[ApiController]
[Route("api/keywords")]
public class KeywordsController : ControllerBase
{
    private readonly IPaperService _paperService;

    public KeywordsController(IPaperService paperService) => _paperService = paperService;

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<KeywordListItemResponse>>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _paperService.GetKeywordsAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<KeywordListItemResponse>>.Ok(result));
    }
}
