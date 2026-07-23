using Grpc.Core;
using PRN232ASM.Pricing.Grpc;

namespace PRN232ASM.PricingService.Services;

public class PricingGrpcService : PaperPricer.PaperPricerBase
{
    private readonly ILogger<PricingGrpcService> _logger;

    public PricingGrpcService(ILogger<PricingGrpcService> logger)
    {
        _logger = logger;
    }

    public override Task<ImpactScoreReply> ComputeImpactScore(
        ImpactScoreRequest request,
        ServerCallContext context)
    {
        var year = request.PublicationYear;
        var age = year > 0 ? Math.Max(0, DateTime.UtcNow.Year - year) : 5;
        var citationBoost = Math.Log10(Math.Max(1, request.CitationCount) + 1) * 25;
        var freshness = Math.Max(0, 20 - age * 2);
        var richness = request.KeywordCount * 1.5 + request.TopicCount * 2 + Math.Min(request.AuthorCount, 8) * 0.5;
        var journalBonus = string.IsNullOrWhiteSpace(request.JournalName) ? 0 : 5;

        var raw = citationBoost + freshness + richness + journalBonus;
        var score = Math.Round(Math.Min(100, raw), 2);
        var tier = score switch
        {
            >= 75 => "Premium",
            >= 45 => "Standard",
            _ => "Emerging"
        };

        var reply = new ImpactScoreReply
        {
            PaperId = request.PaperId,
            Score = score,
            CurrencyLabel = "ImpactPoints",
            Tier = tier,
            Explanation =
                $"citations={request.CitationCount}, age={age}y, keywords={request.KeywordCount}, topics={request.TopicCount} → {tier}"
        };

        _logger.LogInformation("Impact score for {PaperId}: {Score} ({Tier})", request.PaperId, score, tier);
        return Task.FromResult(reply);
    }
}
