namespace SyncService.Application.DTOs;

public class DataSourceResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? LastSyncedAt { get; set; }
}

public class SyncLogResponse
{
    public Guid Id { get; set; }
    public Guid DataSourceId { get; set; }
    public string DataSourceName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int PapersImported { get; set; }
    public string? Errors { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
}
