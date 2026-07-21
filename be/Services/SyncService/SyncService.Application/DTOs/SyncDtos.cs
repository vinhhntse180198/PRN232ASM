namespace SyncService.Application.DTOs;

public record DataSourceDto(
    Guid Id,
    string Name,
    string BaseUrl,
    bool IsEnabled,
    int MaxImportCount,
    DateTime? LastSyncedAt);

public record UpdateDataSourceRequest(
    string? BaseUrl,
    string? ApiKey,
    bool? IsEnabled,
    int? MaxImportCount);

public record SyncLogDto(
    Guid Id,
    Guid DataSourceId,
    string DataSourceName,
    string Status,
    int PapersImported,
    DateTime StartedAt,
    DateTime? CompletedAt,
    string? Message);

public record SyncStatusDto(
    bool IsRunning,
    SyncLogDto? LatestLog,
    IReadOnlyList<DataSourceDto> DataSources);

public record PaperImportRequest(
    string ExternalId,
    string Title,
    string? Abstract,
    int? PublicationYear,
    string? Doi,
    int CitationCount,
    string? JournalName,
    IReadOnlyList<string> AuthorNames,
    IReadOnlyList<string> Keywords,
    IReadOnlyList<string> Topics);
