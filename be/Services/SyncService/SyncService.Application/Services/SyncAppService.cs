using AutoMapper;
using Microsoft.Extensions.Options;
using PRN232ASM.BuildingBlocks.Common.Exceptions;
using PRN232ASM.BuildingBlocks.Contracts.Papers;
using PRN232ASM.BuildingBlocks.EventBus.Outbox;
using SyncService.Application.DTOs;
using SyncService.Application.Interfaces;
using SyncService.Application.Settings;
using SyncService.Domain.Entities;

namespace SyncService.Application.Services;

public class SyncAppService : ISyncService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOpenAlexClient _openAlexClient;
    private readonly IPaperImportClient _paperImportClient;
    private readonly IOutboxWriter _outbox;
    private readonly IMapper _mapper;
    private readonly OpenAlexSettings _openAlexSettings;

    public SyncAppService(
        IUnitOfWork unitOfWork,
        IOpenAlexClient openAlexClient,
        IPaperImportClient paperImportClient,
        IOutboxWriter outbox,
        IMapper mapper,
        IOptions<OpenAlexSettings> openAlexOptions)
    {
        _unitOfWork = unitOfWork;
        _openAlexClient = openAlexClient;
        _paperImportClient = paperImportClient;
        _outbox = outbox;
        _mapper = mapper;
        _openAlexSettings = openAlexOptions.Value;
    }

    public async Task<IReadOnlyList<DataSourceDto>> GetDataSourcesAsync(CancellationToken cancellationToken = default)
    {
        var sources = await _unitOfWork.DataSources.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<DataSourceDto>>(sources);
    }

    public async Task<DataSourceDto> UpdateDataSourceAsync(Guid id, UpdateDataSourceRequest request, CancellationToken cancellationToken = default)
    {
        var source = await _unitOfWork.DataSources.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("DataSource", id);

        if (request.BaseUrl is not null) source.BaseUrl = request.BaseUrl;
        if (request.ApiKey is not null) source.ApiKey = request.ApiKey;
        if (request.IsEnabled.HasValue) source.IsEnabled = request.IsEnabled.Value;
        if (request.MaxImportCount.HasValue) source.MaxImportCount = request.MaxImportCount.Value;
        source.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.DataSources.UpdateAsync(source, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<DataSourceDto>(source);
    }

    public Task<SyncLogDto> TriggerSyncAsync(CancellationToken cancellationToken = default)
        => RunOpenAlexSyncAsync(cancellationToken);

    public async Task<IReadOnlyList<SyncLogDto>> GetLogsAsync(CancellationToken cancellationToken = default)
    {
        var logs = await _unitOfWork.SyncLogs.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<SyncLogDto>>(logs);
    }

    public async Task<SyncStatusDto> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        var latest = await _unitOfWork.SyncLogs.GetLatestAsync(cancellationToken);
        var sources = await GetDataSourcesAsync(cancellationToken);
        var isRunning = latest is not null && latest.Status == "RUNNING" && latest.CompletedAt is null;

        return new SyncStatusDto(
            isRunning,
            latest is null ? null : _mapper.Map<SyncLogDto>(latest),
            sources);
    }

    public async Task<SyncLogDto> RunOpenAlexSyncAsync(CancellationToken cancellationToken = default)
    {
        if (!_openAlexSettings.Enabled)
            throw new InvalidOperationException("OpenAlex sync is disabled. Set OpenAlex__Enabled=true to enable.");

        var source = await _unitOfWork.DataSources.GetByNameAsync("OpenAlex", cancellationToken)
            ?? throw new InvalidOperationException("OpenAlex data source not found.");

        if (!source.IsEnabled)
            throw new InvalidOperationException("OpenAlex data source is disabled.");

        var log = new SyncLog
        {
            Id = Guid.NewGuid(),
            DataSourceId = source.Id,
            Status = "RUNNING",
            PapersImported = 0,
            StartedAt = DateTime.UtcNow,
            DataSource = source
        };

        await _unitOfWork.SyncLogs.AddAsync(log, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var maxCount = Math.Max(1, source.MaxImportCount);
        var imported = 0;
        var skipped = 0;
        var errors = new List<string>();

        try
        {
            var response = await _openAlexClient.FetchWorksAsync(source, maxCount, cancellationToken);

            foreach (var work in response.Results)
            {
                if (imported >= maxCount) break;

                try
                {
                    var request = OpenAlexWorkMapper.ToImportRequest(work);
                    if (request is null) continue;

                    var paperId = await _paperImportClient.GetCreatedPaperIdAsync(request, cancellationToken);
                    if (paperId.HasValue)
                    {
                        imported++;

                        await _outbox.EnqueueAsync(new NewPaperDetectedEvent
                        {
                            PaperId = paperId.Value,
                            Title = request.Title
                        }, cancellationToken);
                    }
                    else
                    {
                        skipped++;
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"{work.Title ?? work.Id}: {ex.Message}");
                }
            }

            log.Status = errors.Count > 0 && imported == 0 ? "FAILED" : "SUCCESS";
            log.PapersImported = imported;
            log.Message = $"Imported {imported}, skipped {skipped} duplicates."
                + (errors.Count > 0 ? $" Errors: {string.Join("; ", errors.Take(5))}" : string.Empty);
            log.CompletedAt = DateTime.UtcNow;

            source.LastSyncedAt = DateTime.UtcNow;
            source.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.DataSources.UpdateAsync(source, cancellationToken);
            await _unitOfWork.SyncLogs.UpdateAsync(log, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<SyncLogDto>(log);
        }
        catch (Exception ex)
        {
            log.Status = "FAILED";
            log.Message = ex.Message;
            log.CompletedAt = DateTime.UtcNow;
            await _unitOfWork.SyncLogs.UpdateAsync(log, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            throw;
        }
    }
}
