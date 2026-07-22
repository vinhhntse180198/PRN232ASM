using PRN232ASM.BuildingBlocks.Contracts.Abstractions;

namespace PRN232ASM.BuildingBlocks.Contracts.Trends;

public class TrendUpdatedEvent : IntegrationEvent
{
    public Guid TopicId { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public Guid KeywordId { get; set; }
    public string Keyword { get; set; } = string.Empty;
    public double GrowthPercent { get; set; }
    public int PaperCount { get; set; }
    public string Period { get; set; } = string.Empty;
}
