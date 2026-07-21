namespace PRN232ASM.TrendService.Domain.Entities;

public class DashboardReport
{
    public Guid Id { get; set; }
    public DateOnly ReportDate { get; set; }
    public int TotalPapers { get; set; }
    public string TopKeyword { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
}
