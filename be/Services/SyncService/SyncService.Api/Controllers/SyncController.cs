using SyncService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SyncService.Api.Controllers;

[ApiController]
[Route("api/sync")]
public class SyncController : ControllerBase
{
    private readonly ISyncService _syncService;
    public SyncController(ISyncService syncService) => _syncService = syncService;

    [HttpGet("status")]
    public async Task<IActionResult> GetStatus(CancellationToken cancellationToken)
        => Ok(new { data = await _syncService.GetStatusAsync(cancellationToken) });

    /// <summary>
    /// Sync papers from OpenAlex (free tier: uses filter, not full-text search).
    /// GET key miễn phí: https://openalex.org/settings/api
    /// </summary>
    [HttpPost("run/{sourceName}")]
    public async Task<IActionResult> RunSync(
        string sourceName,
        [FromQuery] short? year,
        [FromQuery] string? years,
        [FromQuery] int perPage = 100,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new Application.DTOs.SyncRunOptions
            {
                Year = year,
                Years = years,
                PerPage = perPage
            };
            return Ok(new { data = await _syncService.RunSyncAsync(sourceName, options, cancellationToken) });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
