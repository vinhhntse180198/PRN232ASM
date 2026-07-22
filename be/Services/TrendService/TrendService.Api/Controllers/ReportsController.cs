using Microsoft.AspNetCore.Mvc;
using PRN232ASM.BuildingBlocks.Common.Models;
using PRN232ASM.TrendService.Application.Interfaces;

namespace PRN232ASM.TrendService.Api.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public ReportsController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetReports(CancellationToken cancellationToken = default)
    {
        var reports = await _dashboardService.GetReportsAsync(cancellationToken);
        return Ok(ApiResponse<object>.Ok(reports));
    }

    [HttpPost("generate")]
    public async Task<ActionResult<ApiResponse<object>>> GenerateReport(CancellationToken cancellationToken = default)
    {
        var report = await _dashboardService.GenerateDailyReportAsync(cancellationToken);
        return Ok(ApiResponse<object>.Ok(report));
    }
}
