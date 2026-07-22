namespace PRN232ASM.PaperService.Domain.Entities;

public class Bookmark
{
    public Guid UserId { get; set; }
    public Guid PaperId { get; set; }
    public DateTime CreatedAt { get; set; }

    public ResearchPaper Paper { get; set; } = null!;
}
