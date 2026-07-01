using SyncService.Application.DTOs;
using SyncService.Application.Interfaces;
using SyncService.Application.Services;
using SyncService.Application.Settings;
using SyncService.Domain.Entities;
using Microsoft.Extensions.Options;

namespace SyncService.Application.Services;

public class SyncAppService : ISyncService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOpenAlexClient _openAlexClient;
    private readonly IPaperImportClient _paperImportClient;
    private readonly OpenAlexSettings _openAlexSettings;

    public SyncAppService(
        IUnitOfWork unitOfWork,
        IOpenAlexClient openAlexClient,
        IPaperImportClient paperImportClient,
        IOptions<OpenAlexSettings> openAlexOptions)
    {
        _unitOfWork = unitOfWork;
        _openAlexClient = openAlexClient;
        _paperImportClient = paperImportClient;
        _openAlexSettings = openAlexOptions.Value;
    }

    public async Task<IReadOnlyList<SyncLogResponse>> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        var logs = await _unitOfWork.SyncJobs.GetAllLogsAsync(cancellationToken);
        return logs.Select(Map).ToList();
    }

    public Task<SyncLogResponse> RunSyncAsync(string sourceName, CancellationToken cancellationToken = default)
        => RunSyncAsync(sourceName, null, cancellationToken);

    public async Task<SyncLogResponse> RunSyncAsync(
        string sourceName,
        SyncRunOptions? options,
        CancellationToken cancellationToken = default)
    {
        if (!sourceName.Equals("OpenAlex", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Sync for '{sourceName}' is not implemented yet. Only OpenAlex is supported.");

        if (!_openAlexSettings.Enabled)
            throw new InvalidOperationException("OpenAlex sync is disabled (free-only mode).");

        var source = await _unitOfWork.SyncJobs.GetDataSourceByNameAsync("OpenAlex", cancellationToken)
            ?? throw new InvalidOperationException("Data source 'OpenAlex' not found.");

        var perPage = options?.PerPage > 0 ? options.PerPage : _openAlexSettings.DefaultPerPage;
        var years = ResolveYears(options);

        var log = new SyncLog
        {
            Id = Guid.NewGuid(),
            DataSourceId = source.Id,
            Status = "RUNNING",
            PapersImported = 0,
            StartedAt = DateTime.UtcNow,
            DataSource = source
        };

        await _unitOfWork.SyncJobs.AddLogAsync(log, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var errors = new List<string>();
        var imported = 0;
        var skipped = 0;

        try
        {
            foreach (var year in years)
            {
                var response = await _openAlexClient.FetchWorksByYearAsync(year, perPage, cancellationToken);

                foreach (var work in response.Results)
                {
                    try
                    {
                        var request = OpenAlexWorkMapper.ToImportRequest(work);
                        if (request is null) continue;

                        var result = await _paperImportClient.ImportAsync(request, cancellationToken);
                        if (result == PaperImportResult.Created) imported++;
                        else if (result == PaperImportResult.SkippedDuplicate) skipped++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"{work.Title ?? work.Id}: {ex.Message}");
                    }
                }
            }

            log.Status = errors.Count > 0 && imported == 0 ? "FAILED" : "SUCCESS";
            log.PapersImported = imported;
            log.Errors = errors.Count > 0
                ? $"Years [{string.Join(", ", years)}]: imported {imported}, skipped {skipped}. Errors: {string.Join("; ", errors.Take(5))}"
                : $"Years [{string.Join(", ", years)}]: imported {imported}, skipped {skipped} duplicates.";
            log.FinishedAt = DateTime.UtcNow;

            source.LastSyncedAt = DateTime.UtcNow;
            source.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SyncJobs.UpdateDataSourceAsync(source, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Map(log);
        }
        catch (Exception ex)
        {
            log.Status = "FAILED";
            log.Errors = ex.Message;
            log.FinishedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            throw;
        }
    }

    private IReadOnlyList<short> ResolveYears(SyncRunOptions? options)
    {
        if (!string.IsNullOrWhiteSpace(options?.Years))
            return ParseYears(options.Years);

        if (options?.Year is > 0)
            return [options.Year.Value];

        return ParseYears(_openAlexSettings.DefaultYears);
    }

    private static IReadOnlyList<short> ParseYears(string years)
        => years.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(y => short.TryParse(y, out var parsed) ? parsed : (short)0)
            .Where(y => y > 0)
            .Distinct()
            .OrderByDescending(y => y)
            .ToList();

    private static SyncLogResponse Map(SyncLog log) => new()
    {
        Id = log.Id,
        DataSourceId = log.DataSourceId,
        DataSourceName = log.DataSource?.Name ?? string.Empty,
        Status = log.Status,
        PapersImported = log.PapersImported,
        Errors = log.Errors,
        StartedAt = log.StartedAt,
        FinishedAt = log.FinishedAt
    };
}
