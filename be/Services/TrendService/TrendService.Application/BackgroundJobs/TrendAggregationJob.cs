using Microsoft.Extensions.Logging;
using PRN232ASM.TrendService.Application.Interfaces;

namespace PRN232ASM.TrendService.Application.BackgroundJobs;

public class TrendAggregationJob
{
    private readonly ITrendCalculatorService _trendCalculatorService;
    private readonly ILogger<TrendAggregationJob> _logger;

    public TrendAggregationJob(ITrendCalculatorService trendCalculatorService, ILogger<TrendAggregationJob> logger)
    {
        _trendCalculatorService = trendCalculatorService;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation("TrendAggregationJob started at {Time}", DateTime.UtcNow);
        await _trendCalculatorService.RecalculateTrendsAsync();
        _logger.LogInformation("TrendAggregationJob completed at {Time}", DateTime.UtcNow);
    }
}
