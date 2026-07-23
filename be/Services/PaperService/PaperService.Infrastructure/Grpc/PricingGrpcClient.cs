using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PRN232ASM.PaperService.Application.Interfaces;
using PRN232ASM.Pricing.Grpc;

namespace PRN232ASM.PaperService.Infrastructure.Grpc;

public class PricingGrpcClient : IPricingClient, IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly PaperPricer.PaperPricerClient _client;
    private readonly ILogger<PricingGrpcClient> _logger;

    public PricingGrpcClient(IConfiguration configuration, ILogger<PricingGrpcClient> logger)
    {
        _logger = logger;
        var address = configuration["PricingService:Address"] ?? "http://localhost:5007";
        _channel = GrpcChannel.ForAddress(address);
        _client = new PaperPricer.PaperPricerClient(_channel);
    }

    public async Task<ImpactScoreResult> ComputeImpactScoreAsync(
        ImpactScoreQuery query,
        CancellationToken cancellationToken = default)
    {
        var request = new ImpactScoreRequest
        {
            PaperId = query.PaperId.ToString(),
            Title = query.Title ?? string.Empty,
            PublicationYear = query.PublicationYear,
            CitationCount = query.CitationCount,
            KeywordCount = query.KeywordCount,
            TopicCount = query.TopicCount,
            AuthorCount = query.AuthorCount,
            JournalName = query.JournalName ?? string.Empty
        };

        _logger.LogInformation("Calling Pricing gRPC for paper {PaperId}", query.PaperId);
        var reply = await _client.ComputeImpactScoreAsync(request, cancellationToken: cancellationToken);
        return new ImpactScoreResult(
            Guid.Parse(reply.PaperId),
            reply.Score,
            reply.CurrencyLabel,
            reply.Tier,
            reply.Explanation);
    }

    public void Dispose() => _channel.Dispose();
}
