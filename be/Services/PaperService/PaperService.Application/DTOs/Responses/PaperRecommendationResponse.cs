namespace PRN232ASM.PaperService.Application.DTOs.Responses;

public class PaperRecommendationResponse
{
    public Guid PaperId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string JournalName { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public IReadOnlyList<string> Authors { get; set; } = Array.Empty<string>();
    public double Score { get; set; }
    public string Reason { get; set; } = string.Empty;
}
