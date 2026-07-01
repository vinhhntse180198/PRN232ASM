using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using TrendService.Application.DTOs;
using TrendService.Application.Interfaces;
using TrendService.Application.Settings;

namespace TrendService.Infrastructure.External;

public class PaperAnalyticsClient : IPaperAnalyticsClient
{
    private readonly HttpClient _http;

    public PaperAnalyticsClient(HttpClient http, IOptions<PaperServiceSettings> options)
    {
        _http = http;
        _http.BaseAddress = new Uri(options.Value.BaseUrl.TrimEnd('/') + "/");
    }

    public async Task<PaperLibraryAnalyticsDto> GetLibraryAnalyticsAsync(short? year, CancellationToken cancellationToken = default)
    {
        var url = year.HasValue
            ? $"api/papers/analytics?year={year.Value}"
            : "api/papers/analytics";

        using var response = await _http.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var envelope = await response.Content.ReadFromJsonAsync<ApiEnvelope<PaperLibraryAnalyticsDto>>(cancellationToken);
        return envelope?.Data ?? new PaperLibraryAnalyticsDto();
    }

    public async Task<IReadOnlyList<BookmarkKeywordItem>> GetBookmarkKeywordStatsAsync(
        string? bearerToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(bearerToken))
            return [];

        using var request = new HttpRequestMessage(HttpMethod.Get, "api/bookmarks/analytics");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

        using var response = await _http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return [];

        var envelope = await response.Content.ReadFromJsonAsync<ApiEnvelope<List<BookmarkKeywordItem>>>(cancellationToken);
        return envelope?.Data ?? [];
    }

    private sealed class ApiEnvelope<T>
    {
        [JsonPropertyName("data")]
        public T? Data { get; set; }
    }
}
