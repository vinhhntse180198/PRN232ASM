namespace PaperService.Domain.Entities;

public class Author
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Affiliation { get; set; }
    public string? Email { get; set; }
    public string? Orcid { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PaperAuthor> PaperAuthors { get; set; } = [];
}
