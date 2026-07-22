using SyncService.Application.DTOs;
using SyncService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using PRN232ASM.BuildingBlocks.Common.Models;

namespace SyncService.Api.Controllers;

[ApiController]
[Route("api/sync")]
public class SyncController : ControllerBase
{
    private readonly ISyncService _syncService;

    public SyncController(ISyncService syncService) => _syncService = syncService;

    [HttpPost("trigger")]
    public async Task<ActionResult<ApiResponse<SyncLogDto>>> Trigger(CancellationToken cancellationToken)
    {
        var data = await _syncService.TriggerSyncAsync(cancellationToken);
        return Ok(ApiResponse<SyncLogDto>.Ok(data, "Sync triggered."));
    }

    [HttpGet("logs")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<SyncLogDto>>>> GetLogs(CancellationToken cancellationToken)
    {
        var data = await _syncService.GetLogsAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<SyncLogDto>>.Ok(data));
    }

    [HttpGet("status")]
    public async Task<ActionResult<ApiResponse<SyncStatusDto>>> GetStatus(CancellationToken cancellationToken)
    {
        var data = await _syncService.GetStatusAsync(cancellationToken);
        return Ok(ApiResponse<SyncStatusDto>.Ok(data));
    }
}
