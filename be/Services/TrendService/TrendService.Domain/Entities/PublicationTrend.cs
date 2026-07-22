namespace PRN232ASM.TrendService.Domain.Entities;

public class PublicationTrend
{
    public Guid Id { get; set; }
    public string Keyword { get; set; } = string.Empty;
    public int Year { get; set; }
    public int PaperCount { get; set; }
    public Guid? TopicId { get; set; }
}
