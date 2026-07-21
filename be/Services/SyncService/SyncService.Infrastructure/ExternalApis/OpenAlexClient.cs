using System.Text.Json;
using Microsoft.Extensions.Options;
using SyncService.Application.DTOs.OpenAlex;
using SyncService.Application.Interfaces;
using SyncService.Application.Settings;
using SyncService.Domain.Entities;

namespace SyncService.Infrastructure.ExternalApis;

public class OpenAlexClient : IOpenAlexClient
{
    private readonly HttpClient _http;
    private readonly OpenAlexSettings _settings;

    private const string SelectFields =
        "id,doi,title,display_name,abstract_inverted_index,publication_year,cited_by_count,authorships,primary_location,keywords,topics";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public OpenAlexClient(HttpClient http, IOptions<OpenAlexSettings> options)
    {
        _http = http;
        _settings = options.Value;
    }

    public async Task<OpenAlexWorksResponse> FetchWorksAsync(
        DataSource dataSource,
        int maxCount,
        CancellationToken cancellationToken = default)
    {
        if (!dataSource.IsEnabled || !_settings.Enabled)
            throw new InvalidOperationException("OpenAlex API is disabled.");

        var limit = Math.Clamp(maxCount, 1, dataSource.MaxImportCount);
        var perPage = Math.Min(100, limit);
        var years = _settings.DefaultYears
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(y => short.TryParse(y, out var year) ? year : _settings.DefaultYear)
            .Distinct()
            .ToList();

        var allResults = new List<OpenAlexWork>();
        var baseUrl = (string.IsNullOrWhiteSpace(dataSource.BaseUrl) ? _settings.BaseUrl : dataSource.BaseUrl).TrimEnd('/');

        foreach (var year in years)
        {
            if (allResults.Count >= limit) break;

            var remaining = limit - allResults.Count;
            var pageSize = Math.Min(perPage, remaining);

            var queryParts = new List<string>
            {
                $"filter={Uri.EscapeDataString($"publication_year:{year},is_oa:true")}",
                $"sort={Uri.EscapeDataString("cited_by_count:desc")}",
                $"per_page={pageSize}",
                $"select={Uri.EscapeDataString(SelectFields)}"
            };

            var apiKey = string.IsNullOrWhiteSpace(dataSource.ApiKey) ? _settings.ApiKey : dataSource.ApiKey;
            if (!string.IsNullOrWhiteSpace(apiKey))
                queryParts.Add($"api_key={Uri.EscapeDataString(apiKey)}");

            var url = $"{baseUrl}/works?{string.Join('&', queryParts)}";
            using var response = await _http.GetAsync(url, cancellationToken);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"OpenAlex error {(int)response.StatusCode}: {TrimError(body)}");

            var page = JsonSerializer.Deserialize<OpenAlexWorksResponse>(body, JsonOptions) ?? new OpenAlexWorksResponse();
            allResults.AddRange(page.Results.Take(remaining));
        }

        return new OpenAlexWorksResponse { Results = allResults.Take(limit).ToList() };
    }

    private static string TrimError(string body)
        => body.Length > 300 ? body[..300] + "..." : body;
}
