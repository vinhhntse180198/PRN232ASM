namespace PaperService.Domain.Entities;

public class PaperAuthor
{
    public Guid PaperId { get; set; }
    public Guid AuthorId { get; set; }
    public short? AuthorOrder { get; set; }

    public ResearchPaper Paper { get; set; } = null!;
    public Author Author { get; set; } = null!;
}
