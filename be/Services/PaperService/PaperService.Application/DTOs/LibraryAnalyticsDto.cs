namespace PaperService.Application.DTOs;

public class LibraryAnalyticsDto
{
    public int TotalPapers { get; set; }
    public IReadOnlyList<YearCountDto> PapersByYear { get; set; } = [];
    public IReadOnlyList<KeywordYearStatDto> KeywordYearStats { get; set; } = [];
    public IReadOnlyList<OpenAccessYearDto> OpenAccessByYear { get; set; } = [];
}

public class YearCountDto
{
    public short Year { get; set; }
    public int Count { get; set; }
}

public class KeywordYearStatDto
{
    public string Keyword { get; set; } = string.Empty;
    public short Year { get; set; }
    public int PaperCount { get; set; }
    public int CitationSum { get; set; }
}

public class OpenAccessYearDto
{
    public short Year { get; set; }
    public int TotalPapers { get; set; }
    public int OpenAccessPapers { get; set; }
    public decimal OpenAccessPercent { get; set; }
}

public class BookmarkKeywordDto
{
    public string Keyword { get; set; } = string.Empty;
    public int PaperCount { get; set; }
    public int CitationSum { get; set; }
}
