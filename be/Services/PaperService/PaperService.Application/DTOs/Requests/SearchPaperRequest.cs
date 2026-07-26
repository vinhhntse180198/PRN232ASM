namespace PRN232ASM.PaperService.Application.DTOs.Requests;

public class SearchPaperRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Keyword { get; set; }
    public string? Author { get; set; }
    public string? Journal { get; set; }
    public Guid? TopicId { get; set; }
}
