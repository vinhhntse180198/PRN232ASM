namespace TrendService.Domain.Entities;

public class PublicationTrend
{
    public Guid Id { get; set; }
    public Guid? KeywordId { get; set; }
    public Guid? TopicId { get; set; }
    public string? KeywordName { get; set; }
    public short Year { get; set; }
    public int PaperCount { get; set; }
    public int CitationSum { get; set; }
    public decimal? GrowthRate { get; set; }
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
}
