namespace SyncService.Domain.Entities;

public class SyncLog
{
    public Guid Id { get; set; }
    public Guid DataSourceId { get; set; }
    public string Status { get; set; } = "RUNNING";
    public int PapersImported { get; set; }
    public string? Errors { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? FinishedAt { get; set; }

    public DataSource? DataSource { get; set; }
}
