namespace PRN232ASM.PaperService.Domain.Entities;

public class Keyword
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<PaperKeyword> PaperKeywords { get; set; } = new List<PaperKeyword>();
}
