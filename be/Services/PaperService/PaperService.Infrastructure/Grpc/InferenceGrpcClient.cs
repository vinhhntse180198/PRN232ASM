using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PRN232ASM.Inference.Grpc;
using PRN232ASM.PaperService.Application.Interfaces;

namespace PRN232ASM.PaperService.Infrastructure.Grpc;

public class InferenceGrpcClient : IInferenceClient, IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly PaperInference.PaperInferenceClient _client;
    private readonly ILogger<InferenceGrpcClient> _logger;

    public InferenceGrpcClient(IConfiguration configuration, ILogger<InferenceGrpcClient> logger)
    {
        _logger = logger;
        var address = configuration["InferenceService:Address"] ?? "http://localhost:5008";
        _channel = GrpcChannel.ForAddress(address);
        _client = new PaperInference.PaperInferenceClient(_channel);
    }

    public async Task<InsightResult> InferInsightsAsync(
        InsightQuery query,
        CancellationToken cancellationToken = default)
    {
        var request = new InsightRequest
        {
            PaperId = query.PaperId.ToString(),
            Title = query.Title ?? string.Empty,
            AbstractText = query.Abstract ?? string.Empty
        };
        request.ExistingKeywords.AddRange(query.ExistingKeywords);
        request.ExistingTopics.AddRange(query.ExistingTopics);

        _logger.LogInformation("Calling Inference gRPC for paper {PaperId}", query.PaperId);
        var reply = await _client.InferInsightsAsync(request, cancellationToken: cancellationToken);
        return new InsightResult(
            Guid.Parse(reply.PaperId),
            reply.SuggestedKeywords.ToList(),
            reply.SuggestedTopics.ToList(),
            reply.Confidence,
            reply.Summary);
    }

    public void Dispose() => _channel.Dispose();
}
