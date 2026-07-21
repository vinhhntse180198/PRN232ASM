using Microsoft.AspNetCore.Mvc;
using PRN232ASM.BuildingBlocks.Common.Models;
using PRN232ASM.TrendService.Application.Interfaces;

namespace PRN232ASM.TrendService.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetDashboard(CancellationToken cancellationToken = default)
    {
        var dashboard = await _dashboardService.GetDashboardAsync(cancellationToken);
        return Ok(ApiResponse<object>.Ok(dashboard));
    }
}
