namespace PRN232ASM.PaperService.Domain.Entities;

public class PaperKeyword
{
    public Guid PaperId { get; set; }
    public Guid KeywordId { get; set; }

    public ResearchPaper Paper { get; set; } = null!;
    public Keyword Keyword { get; set; } = null!;
}
