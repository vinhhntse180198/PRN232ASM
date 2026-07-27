using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PRN232ASM.Inventory.Grpc;
using PRN232ASM.PaperService.Application.Interfaces;

namespace PRN232ASM.PaperService.Infrastructure.Grpc;

public class InventoryGrpcClient : IInventoryClient, IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly JournalInventory.JournalInventoryClient _client;
    private readonly ILogger<InventoryGrpcClient> _logger;

    public InventoryGrpcClient(IConfiguration configuration, ILogger<InventoryGrpcClient> logger)
    {
        _logger = logger;
        var address = configuration["InventoryService:Address"] ?? "http://localhost:5009";
        _channel = GrpcChannel.ForAddress(address);
        _client = new JournalInventory.JournalInventoryClient(_channel);
    }

    public async Task<JournalCapacityResult> GetCapacityAsync(
        JournalCapacityQuery query,
        CancellationToken cancellationToken = default)
    {
        var request = new CapacityRequest
        {
            JournalId = query.JournalId.ToString(),
            JournalName = query.JournalName ?? string.Empty,
            Year = query.Year,
            PapersInYear = query.PapersInYear,
            TotalPapers = query.TotalPapers
        };

        _logger.LogInformation("Calling Inventory gRPC for journal {JournalId}", query.JournalId);
        var reply = await _client.GetCapacityAsync(request, cancellationToken: cancellationToken);
        return new JournalCapacityResult(
            Guid.Parse(reply.JournalId),
            reply.Year,
            reply.Capacity,
            reply.Used,
            reply.Remaining,
            reply.Status,
            reply.Note);
    }

    public void Dispose() => _channel.Dispose();
}
