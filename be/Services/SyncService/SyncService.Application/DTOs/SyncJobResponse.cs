namespace SyncService.Application.DTOs;

public class SyncJobResponse
{
    public Guid Id { get; set; }
    public string SourceName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? LastRunAt { get; set; }
    public int RecordsSynced { get; set; }
    public DateTime CreatedAt { get; set; }
}
