namespace SyncService.Application.Settings;

public class OpenAlexSettings
{
    public const string SectionName = "OpenAlex";

    public string BaseUrl { get; set; } = "https://api.openalex.org";
    public string? ApiKey { get; set; }
    public bool Enabled { get; set; }
    public short DefaultYear { get; set; } = 2023;
    public string DefaultYears { get; set; } = "2022,2023,2024";
    public int DefaultPerPage { get; set; } = 100;
}
