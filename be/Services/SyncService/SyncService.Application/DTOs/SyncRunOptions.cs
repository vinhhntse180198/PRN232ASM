namespace SyncService.Application.DTOs;

public class SyncRunOptions
{
    public short? Year { get; set; }
    /// <summary>Comma-separated years, e.g. "2022,2023,2024"</summary>
    public string? Years { get; set; }
    public int PerPage { get; set; } = 100;
}