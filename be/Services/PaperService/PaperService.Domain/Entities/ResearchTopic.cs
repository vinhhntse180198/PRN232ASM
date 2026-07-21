namespace PRN232ASM.PaperService.Domain.Entities;

public class ResearchTopic
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<PaperTopic> PaperTopics { get; set; } = new List<PaperTopic>();
}
