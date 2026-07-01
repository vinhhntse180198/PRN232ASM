namespace PaperService.Domain.Entities;

public class Bookmark
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PaperId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ResearchPaper Paper { get; set; } = null!;
}
