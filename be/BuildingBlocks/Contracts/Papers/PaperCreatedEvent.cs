using PRN232ASM.BuildingBlocks.Contracts.Abstractions;

namespace PRN232ASM.BuildingBlocks.Contracts.Papers;

public class PaperCreatedEvent : IntegrationEvent
{
    public Guid PaperId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public Guid? TopicId { get; set; }
    public string? TopicName { get; set; }
    public Guid? JournalId { get; set; }
    public string? JournalName { get; set; }
    public IReadOnlyList<Guid> TopicIds { get; set; } = Array.Empty<Guid>();
    public IReadOnlyList<Guid> KeywordIds { get; set; } = Array.Empty<Guid>();
    public IReadOnlyList<string> Keywords { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> Authors { get; set; } = Array.Empty<string>();
    public string? Url { get; set; }
    public string? PdfUrl { get; set; }
}
