namespace SyncService.Domain.Entities;

public class SyncLog
{
    public Guid Id { get; set; }
    public Guid DataSourceId { get; set; }
    public string Status { get; set; } = "PENDING";
    public int PapersImported { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public string? Message { get; set; }

    public DataSource DataSource { get; set; } = null!;
}
