namespace PRN232ASM.TrendService.Application.DTOs;

public class TrendResponse
{
    public Guid Id { get; set; }
    public string Keyword { get; set; } = string.Empty;
    public int Year { get; set; }
    public int PaperCount { get; set; }
    public Guid? TopicId { get; set; }
}

public class AnalyticsResponse
{
    public int TotalPapers { get; set; }
    public int TotalKeywords { get; set; }
    public int YearFrom { get; set; }
    public int YearTo { get; set; }
    public string TopKeyword { get; set; } = string.Empty;
    public int TopKeywordCount { get; set; }
    public IReadOnlyList<YearCountDto> PapersByYear { get; set; } = Array.Empty<YearCountDto>();
}

public class YearCountDto
{
    public int Year { get; set; }
    public int Count { get; set; }
}

public class KeywordCountDto
{
    public string Keyword { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class DashboardResponse
{
    public IReadOnlyList<YearCountDto> PapersByYear { get; set; } = Array.Empty<YearCountDto>();
    public IReadOnlyList<KeywordCountDto> TopKeywords { get; set; } = Array.Empty<KeywordCountDto>();
    public int TotalPapers { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class ReportResponse
{
    public Guid Id { get; set; }
    public DateOnly ReportDate { get; set; }
    public int TotalPapers { get; set; }
    public string TopKeyword { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
}
