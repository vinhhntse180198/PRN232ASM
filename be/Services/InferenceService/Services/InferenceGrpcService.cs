using System.Text.RegularExpressions;
using Grpc.Core;
using PRN232ASM.Inference.Grpc;

namespace PRN232ASM.InferenceService.Services;

public class InferenceGrpcService : PaperInference.PaperInferenceBase
{
    private static readonly string[] TopicLexicon =
    [
        "machine learning", "deep learning", "neural network", "natural language",
        "computer vision", "bioinformatics", "genomics", "climate", "sustainability",
        "quantum", "blockchain", "cybersecurity", "robotics", "materials",
        "epidemiology", "pharmacology", "education", "economics", "sociology"
    ];

    private readonly ILogger<InferenceGrpcService> _logger;

    public InferenceGrpcService(ILogger<InferenceGrpcService> logger)
    {
        _logger = logger;
    }

    public override Task<InsightReply> InferInsights(InsightRequest request, ServerCallContext context)
    {
        var corpus = $"{request.Title} {request.AbstractText}".ToLowerInvariant();
        var tokens = Regex.Matches(corpus, "[a-z]{4,}")
            .Select(m => m.Value)
            .Where(t => t is not ("that" or "this" or "with" or "from" or "have" or "were" or "been" or "their" or "which" or "into" or "using"))
            .GroupBy(t => t)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .ToList();

        var existingKw = request.ExistingKeywords.Select(Normalize).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var existingTopics = request.ExistingTopics.Select(Normalize).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var suggestedKeywords = tokens
            .Where(t => !existingKw.Contains(t))
            .Take(8)
            .ToList();

        var suggestedTopics = TopicLexicon
            .Where(topic => corpus.Contains(topic) && !existingTopics.Contains(topic))
            .Take(5)
            .ToList();

        if (suggestedTopics.Count == 0 && tokens.Count > 0)
            suggestedTopics.Add(tokens[0]);

        var overlap = request.ExistingKeywords.Count + request.ExistingTopics.Count;
        var confidence = Math.Round(Math.Min(0.95, 0.35 + suggestedKeywords.Count * 0.05 + suggestedTopics.Count * 0.08 + Math.Min(overlap, 6) * 0.03), 3);

        var reply = new InsightReply
        {
            PaperId = request.PaperId,
            Confidence = confidence,
            Summary = suggestedTopics.Count > 0
                ? $"Heuristic inference suggests focus on: {string.Join(", ", suggestedTopics.Take(3))}."
                : "Limited lexical signal; suggestions derived from frequent title/abstract tokens."
        };
        reply.SuggestedKeywords.AddRange(suggestedKeywords);
        reply.SuggestedTopics.AddRange(suggestedTopics);

        _logger.LogInformation(
            "Inference for {PaperId}: {Kw} keywords, {Topics} topics, confidence {Confidence}",
            request.PaperId,
            suggestedKeywords.Count,
            suggestedTopics.Count,
            confidence);

        return Task.FromResult(reply);
    }

    private static string Normalize(string? value) => (value ?? string.Empty).Trim().ToLowerInvariant();
}
