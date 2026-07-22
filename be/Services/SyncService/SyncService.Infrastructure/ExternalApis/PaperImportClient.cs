using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SyncService.Application.DTOs;
using SyncService.Application.Interfaces;
using SyncService.Application.Settings;

namespace SyncService.Infrastructure.ExternalApis;

public class PaperImportClient : IPaperImportClient
{
    private readonly HttpClient _http;
    private readonly ILogger<PaperImportClient> _logger;

    public PaperImportClient(HttpClient http, IOptions<PaperServiceSettings> options, ILogger<PaperImportClient> logger)
    {
        _http = http;
        _logger = logger;
        _http.BaseAddress = new Uri(options.Value.BaseUrl.TrimEnd('/') + "/");
    }

    public async Task<PaperImportResult> ImportAsync(PaperImportRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/papers/import", request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Conflict)
                return PaperImportResult.SkippedDuplicate;

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("Paper import failed for {ExternalId}: {Status} {Body}", request.ExternalId, response.StatusCode, body);
                return PaperImportResult.Failed;
            }

            return PaperImportResult.Created;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Paper import request failed for {ExternalId}", request.ExternalId);
            return PaperImportResult.Failed;
        }
    }
}
