using SyncService.Application.DTOs;
using SyncService.Application.Interfaces;
using SyncService.Application.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace SyncService.Application.Services;

public class OpenAlexSearchService : IOpenAlexSearchService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(15);

    private readonly IOpenAlexClient _openAlexClient;
    private readonly IPaperImportClient _paperImportClient;
    private readonly IMemoryCache _cache;
    private readonly OpenAlexSettings _settings;

    public OpenAlexSearchService(
        IOpenAlexClient openAlexClient,
        IPaperImportClient paperImportClient,
        IMemoryCache cache,
        IOptions<OpenAlexSettings> openAlexOptions)
    {
        _openAlexClient = openAlexClient;
        _paperImportClient = paperImportClient;
        _cache = cache;
        _settings = openAlexOptions.Value;
    }

    private void EnsureEnabled()
    {
        if (!_settings.Enabled)
            throw new InvalidOperationException("OpenAlex đã tắt — chỉ dùng thư viện local miễn phí.");
    }

    public async Task<OpenAlexSearchResponse> SearchAsync(
        string query,
        short? year,
        bool openAccessOnly,
        bool paywalledOnly,
        int page,
        int perPage,
        CancellationToken cancellationToken = default)
    {
        EnsureEnabled();
        if (string.IsNullOrWhiteSpace(query))
            throw new InvalidOperationException("Search query is required.");

        var cacheKey = $"search:{query.Trim().ToLowerInvariant()}:{year}:{openAccessOnly}:{paywalledOnly}:{page}:{perPage}";
        if (_cache.TryGetValue(cacheKey, out OpenAlexSearchResponse? cachedSearch) && cachedSearch is not null)
            return cachedSearch;

        var response = await _openAlexClient.SearchWorksAsync(
            query.Trim(),
            year,
            openAccessOnly,
            paywalledOnly,
            page < 1 ? 1 : page,
            perPage,
            cancellationToken);

        var items = response.Results
            .Select(OpenAlexWorkMapper.ToSearchResult)
            .Where(p => p is not null)
            .Cast<OpenAlexPaperResult>()
            .Where(p =>
                (!openAccessOnly && !paywalledOnly) ||
                (openAccessOnly && p.CanReadInApp) ||
                (paywalledOnly && p.IsPaywalled))
            .ToList();

        var result = new OpenAlexSearchResponse
        {
            Items = items,
            Total = response.Meta?.Count ?? items.Count,
            Page = page < 1 ? 1 : page,
            PageSize = perPage
        };

        _cache.Set(cacheKey, result, CacheTtl);
        return result;
    }

    public async Task<OpenAlexSearchResponse> BrowseOpenAccessAsync(
        short? year,
        int page,
        int perPage,
        CancellationToken cancellationToken = default)
    {
        EnsureEnabled();
        var response = await _openAlexClient.BrowseOpenAccessWorksAsync(
            year,
            page < 1 ? 1 : page,
            perPage,
            cancellationToken);

        var items = response.Results
            .Select(OpenAlexWorkMapper.ToSearchResult)
            .Where(p => p is not null)
            .Cast<OpenAlexPaperResult>()
            .Where(p => p.CanReadInApp)
            .ToList();

        return new OpenAlexSearchResponse
        {
            Items = items,
            Total = response.Meta?.Count ?? items.Count,
            Page = page < 1 ? 1 : page,
            PageSize = perPage
        };
    }

    public async Task<OpenAlexSearchResponse> BrowsePaywalledAsync(
        short? year,
        int page,
        int perPage,
        CancellationToken cancellationToken = default)
    {
        EnsureEnabled();
        var response = await _openAlexClient.BrowsePaywalledWorksAsync(
            year,
            page < 1 ? 1 : page,
            perPage,
            cancellationToken);

        var items = response.Results
            .Select(OpenAlexWorkMapper.ToSearchResult)
            .Where(p => p is not null)
            .Cast<OpenAlexPaperResult>()
            .Where(p => p.IsPaywalled)
            .ToList();

        return new OpenAlexSearchResponse
        {
            Items = items,
            Total = response.Meta?.Count ?? items.Count,
            Page = page < 1 ? 1 : page,
            PageSize = perPage
        };
    }

    public async Task<OpenAlexSearchResponse> BrowseAllAsync(
        short? year,
        int page,
        int perPage,
        CancellationToken cancellationToken = default)
    {
        EnsureEnabled();
        var cacheKey = $"browse-all:{year}:{page}:{perPage}";
        if (_cache.TryGetValue(cacheKey, out OpenAlexSearchResponse? cachedBrowse) && cachedBrowse is not null)
            return cachedBrowse;

        var response = await _openAlexClient.BrowseAllWorksAsync(
            year,
            page < 1 ? 1 : page,
            perPage,
            cancellationToken);

        var items = response.Results
            .Select(OpenAlexWorkMapper.ToSearchResult)
            .Where(p => p is not null)
            .Cast<OpenAlexPaperResult>()
            .ToList();

        var result = new OpenAlexSearchResponse
        {
            Items = items,
            Total = response.Meta?.Count ?? items.Count,
            Page = page < 1 ? 1 : page,
            PageSize = perPage
        };

        _cache.Set(cacheKey, result, CacheTtl);
        return result;
    }

    public async Task<OpenAlexPaperResult> GetWorkAsync(string openAlexId, CancellationToken cancellationToken = default)
    {
        EnsureEnabled();
        if (string.IsNullOrWhiteSpace(openAlexId))
            throw new InvalidOperationException("OpenAlex work id is required.");

        var work = await _openAlexClient.GetWorkByIdAsync(openAlexId, cancellationToken)
            ?? throw new InvalidOperationException("Work not found on OpenAlex.");

        return OpenAlexWorkMapper.ToSearchResult(work)
            ?? throw new InvalidOperationException("Could not map work.");
    }

    public async Task<OpenAlexPaperResult> ImportToLibraryAsync(string openAlexId, CancellationToken cancellationToken = default)
    {
        EnsureEnabled();
        if (string.IsNullOrWhiteSpace(openAlexId))
            throw new InvalidOperationException("OpenAlex work id is required.");

        var work = await _openAlexClient.GetWorkByIdAsync(openAlexId, cancellationToken)
            ?? throw new InvalidOperationException("Work not found on OpenAlex.");

        var importRequest = OpenAlexWorkMapper.ToImportRequest(work)
            ?? throw new InvalidOperationException("Could not map work for import.");

        _ = await _paperImportClient.ImportAsync(importRequest, cancellationToken);

        return OpenAlexWorkMapper.ToSearchResult(work)
            ?? throw new InvalidOperationException("Could not map imported work.");
    }
}
