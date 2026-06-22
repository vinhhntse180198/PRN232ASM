namespace PaperService.Domain.Entities;

public class PaperKeyword
{
    public Guid PaperId { get; set; }
    public ResearchPaper Paper { get; set; } = null!;
    public Guid KeywordId { get; set; }
    public Keyword Keyword { get; set; } = null!;
}
