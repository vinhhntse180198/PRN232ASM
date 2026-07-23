using Grpc.Core;
using PRN232ASM.Inventory.Grpc;

namespace PRN232ASM.InventoryService.Services;

public class InventoryGrpcService : JournalInventory.JournalInventoryBase
{
    private readonly ILogger<InventoryGrpcService> _logger;

    public InventoryGrpcService(ILogger<InventoryGrpcService> logger)
    {
        _logger = logger;
    }

    public override Task<CapacityReply> GetCapacity(CapacityRequest request, ServerCallContext context)
    {
        var year = request.Year > 0 ? request.Year : DateTime.UtcNow.Year;
        // Capacity derived from journal identity + historical volume (real counts from Paper DB).
        var hash = Math.Abs((request.JournalName ?? request.JournalId).GetHashCode(StringComparison.OrdinalIgnoreCase));
        var baseCapacity = 20 + hash % 40; // 20..59 slots / year
        var volumeBonus = Math.Min(30, request.TotalPapers / 5);
        var capacity = baseCapacity + volumeBonus;
        var used = Math.Max(0, request.PapersInYear);
        var remaining = Math.Max(0, capacity - used);
        var fill = capacity == 0 ? 1 : (double)used / capacity;
        var status = fill switch
        {
            >= 1 => "Full",
            >= 0.8 => "Tight",
            >= 0.4 => "Available",
            _ => "Open"
        };

        var reply = new CapacityReply
        {
            JournalId = request.JournalId,
            Year = year,
            Capacity = capacity,
            Used = used,
            Remaining = remaining,
            Status = status,
            Note = $"{request.JournalName}: {used}/{capacity} papers in {year} (from Paper DB counts)"
        };

        _logger.LogInformation(
            "Inventory {JournalId} {Year}: used={Used} capacity={Capacity} status={Status}",
            request.JournalId,
            year,
            used,
            capacity,
            status);

        return Task.FromResult(reply);
    }
}
