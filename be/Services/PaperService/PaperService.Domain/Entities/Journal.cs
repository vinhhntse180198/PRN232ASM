namespace PRN232ASM.PaperService.Domain.Entities;

public class Journal
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Issn { get; set; }
    public string? Publisher { get; set; }

    public ICollection<ResearchPaper> Papers { get; set; } = new List<ResearchPaper>();
}
