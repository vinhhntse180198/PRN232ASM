namespace PaperService.Application.DTOs;

public class AuthorResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Affiliation { get; set; }
    public short? AuthorOrder { get; set; }
}
