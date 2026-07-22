namespace PRN232ASM.PaperService.Domain.Entities;

public class Author
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Affiliation { get; set; }
    public string? Email { get; set; }

    public ICollection<PaperAuthor> PaperAuthors { get; set; } = new List<PaperAuthor>();
}
