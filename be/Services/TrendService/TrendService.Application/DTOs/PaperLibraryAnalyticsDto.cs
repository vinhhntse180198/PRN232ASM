namespace TrendService.Application.DTOs;

public class PaperLibraryAnalyticsDto
{
    public int TotalPapers { get; set; }
    public List<YearCountItem> PapersByYear { get; set; } = [];
    public List<KeywordYearStatItem> KeywordYearStats { get; set; } = [];
    public List<OpenAccessYearItem> OpenAccessByYear { get; set; } = [];
}

public class YearCountItem
{
    public short Year { get; set; }
    public int Count { get; set; }
}

public class KeywordYearStatItem
{
    public string Keyword { get; set; } = string.Empty;
    public short Year { get; set; }
    public int PaperCount { get; set; }
    public int CitationSum { get; set; }
}

public class OpenAccessYearItem
{
    public short Year { get; set; }
    public int TotalPapers { get; set; }
    public int OpenAccessPapers { get; set; }
    public decimal OpenAccessPercent { get; set; }
}

public class BookmarkKeywordItem
{
    public string Keyword { get; set; } = string.Empty;
    public int PaperCount { get; set; }
    public int CitationSum { get; set; }
}
