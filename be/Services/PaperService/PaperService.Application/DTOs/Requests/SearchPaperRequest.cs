namespace PaperService.Application.DTOs.Requests;

public class SearchPaperRequest
{
    public string? Keyword { get; set; }
    public string? Author { get; set; }
    public string? Journal { get; set; }
    public Guid? TopicId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
