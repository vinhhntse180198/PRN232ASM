namespace PaperService.Application.DTOs;

public class SearchPaperRequest
{
    public string? Query { get; set; }
    public string? Keyword { get; set; }
    public string? Doi { get; set; }
    public short? Year { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
