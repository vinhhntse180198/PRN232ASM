namespace PaperService.Domain.Entities;

public class Journal
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Issn { get; set; }
    public string? Publisher { get; set; }
    public decimal? ImpactFactor { get; set; }
    public string? WebsiteUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ResearchPaper> Papers { get; set; } = [];
}
