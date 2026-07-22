using SyncService.Application.DTOs;
using SyncService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using PRN232ASM.BuildingBlocks.Common.Models;

namespace SyncService.Api.Controllers;

[ApiController]
[Route("api/datasources")]
public class DataSourcesController : ControllerBase
{
    private readonly ISyncService _syncService;

    public DataSourcesController(ISyncService syncService) => _syncService = syncService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DataSourceDto>>>> GetAll(CancellationToken cancellationToken)
    {
        var data = await _syncService.GetDataSourcesAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<DataSourceDto>>.Ok(data));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<DataSourceDto>>> Update(
        Guid id,
        [FromBody] UpdateDataSourceRequest request,
        CancellationToken cancellationToken)
    {
        var data = await _syncService.UpdateDataSourceAsync(id, request, cancellationToken);
        return Ok(ApiResponse<DataSourceDto>.Ok(data, "Data source updated."));
    }
}
