namespace PaperService.Domain.Entities;

public class PaperTopic
{
    public Guid PaperId { get; set; }
    public ResearchPaper Paper { get; set; } = null!;
    public Guid TopicId { get; set; }
    public ResearchTopic Topic { get; set; } = null!;
}
