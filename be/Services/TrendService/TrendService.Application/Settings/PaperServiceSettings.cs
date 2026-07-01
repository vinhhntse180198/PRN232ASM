namespace TrendService.Application.Settings;

public class PaperServiceSettings
{
    public const string SectionName = "PaperService";
    public string BaseUrl { get; set; } = "http://localhost:5002";
}
