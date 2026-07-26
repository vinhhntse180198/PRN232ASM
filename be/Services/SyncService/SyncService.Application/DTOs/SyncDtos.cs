using System.Text.Json.Serialization;

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
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("abstract")] string? Abstract,
    [property: JsonPropertyName("doi")] string? Doi,
    [property: JsonPropertyName("publicationYear")] int? PublicationYear,
    [property: JsonPropertyName("citationCount")] int CitationCount,
    [property: JsonPropertyName("journalName")] string? JournalName,
    [property: JsonPropertyName("authors")] IReadOnlyList<string> Authors,
    [property: JsonPropertyName("keywords")] IReadOnlyList<string> Keywords,
    [property: JsonPropertyName("topics")] IReadOnlyList<string> Topics,
    [property: JsonPropertyName("url")] string? Url = null,
    [property: JsonPropertyName("pdfUrl")] string? PdfUrl = null);
