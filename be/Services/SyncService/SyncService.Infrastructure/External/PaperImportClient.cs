using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using SyncService.Application.DTOs;
using SyncService.Application.Interfaces;
using SyncService.Application.Settings;

namespace SyncService.Infrastructure.External;

public class PaperImportClient : IPaperImportClient
{
    private readonly HttpClient _http;

    public PaperImportClient(HttpClient http, IOptions<PaperServiceSettings> options)
    {
        _http = http;
        _http.BaseAddress = new Uri(options.Value.BaseUrl.TrimEnd('/') + "/");
    }

    public async Task<PaperImportResult> ImportAsync(PaperImportRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await _http.PostAsJsonAsync("api/papers", request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
            return PaperImportResult.Created;

        if (body.Contains("DOI already exists", StringComparison.OrdinalIgnoreCase))
            return PaperImportResult.SkippedDuplicate;

        throw new InvalidOperationException($"PaperService error {(int)response.StatusCode}: {body}");
    }
}
