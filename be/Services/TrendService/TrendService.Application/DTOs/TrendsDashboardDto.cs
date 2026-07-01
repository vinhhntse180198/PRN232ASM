namespace TrendService.Application.DTOs;

public class TrendsDashboardDto
{
    public int TotalPapers { get; set; }
    public short? FilterYear { get; set; }
    public DateTime? LastRefreshedAt { get; set; }
    public IReadOnlyList<KeywordTrendItemDto> TopKeywords { get; set; } = [];
    public IReadOnlyList<KeywordTrendItemDto> CitationsByKeyword { get; set; } = [];
    public IReadOnlyList<YearCountItem> PapersByYear { get; set; } = [];
    public IReadOnlyList<OpenAccessYearItem> OpenAccessByYear { get; set; } = [];
    public IReadOnlyList<BookmarkKeywordItem> BookmarkKeywords { get; set; } = [];
}

public class KeywordTrendItemDto
{
    public string Keyword { get; set; } = string.Empty;
    public short Year { get; set; }
    public int PaperCount { get; set; }
    public int CitationSum { get; set; }
    public decimal? GrowthRate { get; set; }
    public string TrendBadge { get; set; } = "stable";
}

public class TrendRefreshResultDto
{
    public int TrendsUpdated { get; set; }
    public DateTime CalculatedAt { get; set; }
}
