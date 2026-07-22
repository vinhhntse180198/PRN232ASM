namespace PRN232ASM.PaperService.Domain.Entities;

public class PaperTopic
{
    public Guid PaperId { get; set; }
    public Guid TopicId { get; set; }

    public ResearchPaper Paper { get; set; } = null!;
    public ResearchTopic Topic { get; set; } = null!;
}
