namespace TrendService.Application.DTOs;

public class CreateTrendRequest
{
    public Guid? KeywordId { get; set; }
    public string? KeywordName { get; set; }
    public Guid? TopicId { get; set; }
    public short Year { get; set; }
    public int PaperCount { get; set; }
    public int CitationSum { get; set; }
    public decimal? GrowthRate { get; set; }
}
