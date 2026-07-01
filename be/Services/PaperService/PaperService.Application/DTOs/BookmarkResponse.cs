namespace PaperService.Application.DTOs;

public class BookmarkResponse
{
    public Guid Id { get; set; }
    public Guid PaperId { get; set; }
    public DateTime CreatedAt { get; set; }
    public PaperResponse Paper { get; set; } = null!;
}
