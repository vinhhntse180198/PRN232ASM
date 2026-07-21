using PRN232ASM.BuildingBlocks.Contracts.Abstractions;

namespace PRN232ASM.BuildingBlocks.Contracts.Papers;

public class PaperImportedEvent : IntegrationEvent
{
    public Guid PaperId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string SourceName { get; set; } = string.Empty;
    public Guid? JournalId { get; set; }
    public List<Guid> TopicIds { get; set; } = [];
    public List<Guid> KeywordIds { get; set; } = [];
}
