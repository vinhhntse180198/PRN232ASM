using System.Text.Json;
using Microsoft.Extensions.Options;
using SyncService.Application.DTOs.OpenAlex;
using SyncService.Application.Interfaces;
using SyncService.Application.Settings;

namespace SyncService.Infrastructure.External;

public class OpenAlexClient : IOpenAlexClient
{
    private readonly HttpClient _http;
    private readonly OpenAlexSettings _settings;

    private const string SelectFields =
        "id,doi,title,display_name,abstract_inverted_index,publication_year,publication_date,cited_by_count,authorships,primary_location,open_access,keywords,topics";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public OpenAlexClient(HttpClient http, IOptions<OpenAlexSettings> options)
    {
        _http = http;
        _settings = options.Value;
    }

    private void EnsureEnabled()
    {
        if (!_settings.Enabled)
            throw new InvalidOperationException("OpenAlex API is disabled (free-only mode). Use the local paper library.");
    }

    public Task<OpenAlexWorksResponse> FetchWorksByYearAsync(short year, int perPage, CancellationToken cancellationToken = default)
    {
        EnsureEnabled();
        var pageSize = Math.Clamp(perPage, 1, 100);
        var queryParts = new List<string>
        {
            $"filter={Uri.EscapeDataString($"publication_year:{year},is_oa:true,has_fulltext:true")}",
            $"sort={Uri.EscapeDataString("cited_by_count:desc")}",
            $"per_page={pageSize}",
            $"select={Uri.EscapeDataString(SelectFields)}"
        };
        AppendApiKey(queryParts);
        return GetWorksListAsync(queryParts, cancellationToken);
    }

    public Task<OpenAlexWorksResponse> BrowseOpenAccessWorksAsync(
        short? year,
        int page,
        int perPage,
        CancellationToken cancellationToken = default)
    {
        EnsureEnabled();
        var pageSize = Math.Clamp(perPage, 1, 25);
        var safePage = page < 1 ? 1 : page;
        var filterParts = new List<string> { "is_oa:true", "has_fulltext:true" };
        if (year is > 0)
            filterParts.Add($"publication_year:{year}");

        var queryParts = new List<string>
        {
            $"filter={Uri.EscapeDataString(string.Join(',', filterParts))}",
            $"sort={Uri.EscapeDataString("cited_by_count:desc")}",
            $"page={safePage}",
            $"per_page={pageSize}",
            $"select={Uri.EscapeDataString(SelectFields)}"
        };
        AppendApiKey(queryParts);
        return GetWorksListAsync(queryParts, cancellationToken);
    }

    public Task<OpenAlexWorksResponse> BrowsePaywalledWorksAsync(
        short? year,
        int page,
        int perPage,
        CancellationToken cancellationToken = default)
    {
        EnsureEnabled();
        var pageSize = Math.Clamp(perPage, 1, 25);
        var safePage = page < 1 ? 1 : page;
        var filterParts = new List<string> { "is_oa:false" };
        if (year is > 0)
            filterParts.Add($"publication_year:{year}");

        var queryParts = new List<string>
        {
            $"filter={Uri.EscapeDataString(string.Join(',', filterParts))}",
            $"sort={Uri.EscapeDataString("cited_by_count:desc")}",
            $"page={safePage}",
            $"per_page={pageSize}",
            $"select={Uri.EscapeDataString(SelectFields)}"
        };
        AppendApiKey(queryParts);
        return GetWorksListAsync(queryParts, cancellationToken);
    }

    public Task<OpenAlexWorksResponse> BrowseAllWorksAsync(
        short? year,
        int page,
        int perPage,
        CancellationToken cancellationToken = default)
    {
        EnsureEnabled();
        var pageSize = Math.Clamp(perPage, 1, 25);
        var safePage = page < 1 ? 1 : page;
        var queryParts = new List<string>();
        if (year is > 0)
            queryParts.Add($"filter={Uri.EscapeDataString($"publication_year:{year}")}");

        queryParts.Add($"sort={Uri.EscapeDataString("cited_by_count:desc")}");
        queryParts.Add($"page={safePage}");
        queryParts.Add($"per_page={pageSize}");
        queryParts.Add($"select={Uri.EscapeDataString(SelectFields)}");
        AppendApiKey(queryParts);
        return GetWorksListAsync(queryParts, cancellationToken);
    }

    public Task<OpenAlexWorksResponse> SearchWorksAsync(
        string query,
        short? year,
        bool openAccessOnly,
        bool paywalledOnly,
        int page,
        int perPage,
        CancellationToken cancellationToken = default)
    {
        EnsureEnabled();
        var pageSize = Math.Clamp(perPage, 1, 25);
        var safePage = page < 1 ? 1 : page;
        var trimmedQuery = query.Trim();
        var filterParts = new List<string>
        {
            $"title_and_abstract.search:{trimmedQuery}"
        };
        if (year is > 0)
            filterParts.Add($"publication_year:{year}");
        if (openAccessOnly)
            filterParts.Add("is_oa:true");
        if (paywalledOnly)
            filterParts.Add("is_oa:false");

        var queryParts = new List<string>
        {
            $"filter={Uri.EscapeDataString(string.Join(',', filterParts))}",
            $"sort={Uri.EscapeDataString("relevance_score:desc")}",
            $"page={safePage}",
            $"per_page={pageSize}",
            $"select={Uri.EscapeDataString(SelectFields)}"
        };

        AppendApiKey(queryParts);
        return GetWorksListAsync(queryParts, cancellationToken);
    }

    public async Task<OpenAlexWork?> GetWorkByIdAsync(string openAlexId, CancellationToken cancellationToken = default)
    {
        EnsureEnabled();
        var id = openAlexId.Trim().TrimStart('/');
        if (id.Contains('/'))
            id = id[(id.LastIndexOf('/') + 1)..];

        var queryParts = new List<string>
        {
            $"select={Uri.EscapeDataString(SelectFields)}"
        };
        AppendApiKey(queryParts);

        var baseUrl = _settings.BaseUrl.TrimEnd('/');
        var url = $"{baseUrl}/works/{id}?{string.Join('&', queryParts)}";

        using var response = await _http.GetAsync(url, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"OpenAlex error {(int)response.StatusCode}: {TrimError(body)}");

        return JsonSerializer.Deserialize<OpenAlexWork>(body, JsonOptions);
    }

    private async Task<OpenAlexWorksResponse> GetWorksListAsync(List<string> queryParts, CancellationToken cancellationToken)
    {
        var baseUrl = _settings.BaseUrl.TrimEnd('/');
        var url = $"{baseUrl}/works?{string.Join('&', queryParts)}";

        using var response = await _http.GetAsync(url, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"OpenAlex error {(int)response.StatusCode}: {TrimError(body)}");

        return JsonSerializer.Deserialize<OpenAlexWorksResponse>(body, JsonOptions) ?? new OpenAlexWorksResponse();
    }

    private void AppendApiKey(List<string> queryParts)
    {
        if (!_settings.Enabled || string.IsNullOrWhiteSpace(_settings.ApiKey))
            return;
        queryParts.Add($"api_key={Uri.EscapeDataString(_settings.ApiKey)}");
    }

    private static string TrimError(string body)
        => body.Length > 300 ? body[..300] + "..." : body;
}
