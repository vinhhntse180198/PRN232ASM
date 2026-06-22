using System.Security.Claims;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaperService.Application.DTOs.Responses;
using PaperService.Application.Interfaces;

namespace PaperService.Api.Controllers;

[ApiController]
[Route("api/journals")]
public class JournalsController : ControllerBase
{
    private readonly IPaperService _paperService;

    public JournalsController(IPaperService paperService) => _paperService = paperService;

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<JournalListItemResponse>>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _paperService.GetJournalsAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<JournalListItemResponse>>.Ok(result));
    }
}
