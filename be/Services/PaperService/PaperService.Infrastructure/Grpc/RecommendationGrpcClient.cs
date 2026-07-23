using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PRN232ASM.PaperService.Application.Interfaces;
using PRN232ASM.Recommendation.Grpc;

namespace PRN232ASM.PaperService.Infrastructure.Grpc;

public class RecommendationGrpcClient : IRecommendationClient, IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly PaperRecommender.PaperRecommenderClient _client;
    private readonly ILogger<RecommendationGrpcClient> _logger;

    public RecommendationGrpcClient(IConfiguration configuration, ILogger<RecommendationGrpcClient> logger)
    {
        _logger = logger;
        var address = configuration["RecommendationService:Address"] ?? "http://localhost:5006";
        _channel = GrpcChannel.ForAddress(address);
        _client = new PaperRecommender.PaperRecommenderClient(_channel);
    }

    public async Task<IReadOnlyList<RecommendationResult>> GetRecommendationsAsync(
        RecommendationQuery query,
        CancellationToken cancellationToken = default)
    {
        var request = new RecommendRequest
        {
            SourcePaperId = query.SourcePaperId.ToString(),
            Limit = query.Limit,
            SourceJournalName = query.SourceJournalName ?? string.Empty,
            SourceYear = query.SourceYear
        };

        request.SourceKeywords.AddRange(query.SourceKeywords);
        request.SourceTopics.AddRange(query.SourceTopics);

        foreach (var candidate in query.Candidates)
        {
            var item = new PaperCandidate
            {
                PaperId = candidate.PaperId.ToString(),
                Title = candidate.Title ?? string.Empty,
                PublicationYear = candidate.PublicationYear,
                JournalName = candidate.JournalName ?? string.Empty
            };
            item.Keywords.AddRange(candidate.Keywords);
            item.Topics.AddRange(candidate.Topics);
            request.Candidates.Add(item);
        }

        _logger.LogInformation(
            "Calling Recommendation gRPC for paper {PaperId} with {Count} candidates",
            query.SourcePaperId,
            query.Candidates.Count);

        var reply = await _client.GetRecommendationsAsync(request, cancellationToken: cancellationToken);

        return reply.Items
            .Select(x => new RecommendationResult(
                Guid.Parse(x.PaperId),
                x.Score,
                x.Reason))
            .ToList();
    }

    public void Dispose() => _channel.Dispose();
}
