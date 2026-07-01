namespace TrendService.Application.Services;

internal static class TrendBadgeHelper
{
    public static string FromGrowthRate(decimal? growthRate)
    {
        if (growthRate is > 5m) return "rising";
        if (growthRate is < -5m) return "falling";
        return "stable";
    }

    public static decimal? ComputeGrowthRate(int current, int previous)
    {
        if (previous <= 0)
            return current > 0 ? 100m : 0m;

        return Math.Round((decimal)(current - previous) / previous * 100m, 2);
    }
}
