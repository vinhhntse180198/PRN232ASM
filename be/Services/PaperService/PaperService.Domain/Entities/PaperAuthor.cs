namespace PaperService.Domain.Entities;

public class PaperAuthor
{
    public Guid PaperId { get; set; }
    public ResearchPaper Paper { get; set; } = null!;
    public Guid AuthorId { get; set; }
    public Author Author { get; set; } = null!;
    public short AuthorOrder { get; set; }
}
