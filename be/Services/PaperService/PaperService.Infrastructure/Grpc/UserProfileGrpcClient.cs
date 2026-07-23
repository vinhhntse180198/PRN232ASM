using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PRN232ASM.PaperService.Application.Interfaces;
using Proto = PRN232ASM.UserProfile.Grpc;

namespace PRN232ASM.PaperService.Infrastructure.Grpc;

public class UserProfileGrpcClient : IUserProfileClient, IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly Proto.ReaderProfiler.ReaderProfilerClient _client;
    private readonly ILogger<UserProfileGrpcClient> _logger;

    public UserProfileGrpcClient(IConfiguration configuration, ILogger<UserProfileGrpcClient> logger)
    {
        _logger = logger;
        var address = configuration["UserProfileService:Address"] ?? "http://localhost:5010";
        _channel = GrpcChannel.ForAddress(address);
        _client = new Proto.ReaderProfiler.ReaderProfilerClient(_channel);
    }

    public async Task<ReadingProfileResult> BuildReadingProfileAsync(
        ReadingProfileQuery query,
        CancellationToken cancellationToken = default)
    {
        var request = new Proto.ReadingProfileRequest
        {
            UserId = query.UserId.ToString()
        };

        foreach (var bm in query.Bookmarks)
        {
            var item = new Proto.BookmarkedPaperSignal
            {
                PaperId = bm.PaperId.ToString(),
                Title = bm.Title ?? string.Empty,
                JournalName = bm.JournalName ?? string.Empty,
                PublicationYear = bm.PublicationYear
            };
            item.Keywords.AddRange(bm.Keywords);
            item.Topics.AddRange(bm.Topics);
            request.Bookmarks.Add(item);
        }

        request.FollowedKeywords.AddRange(query.FollowedKeywords);
        request.FollowedTopics.AddRange(query.FollowedTopics);
        request.FollowedJournals.AddRange(query.FollowedJournals);

        _logger.LogInformation("Calling UserProfile gRPC for user {UserId}", query.UserId);
        var reply = await _client.BuildReadingProfileAsync(request, cancellationToken: cancellationToken);

        return new ReadingProfileResult(
            Guid.Parse(reply.UserId),
            reply.BookmarkCount,
            reply.TopKeywords.Select(x => new InterestItemResult(x.Name, x.Weight)).ToList(),
            reply.TopTopics.Select(x => new InterestItemResult(x.Name, x.Weight)).ToList(),
            reply.TopJournals.Select(x => new InterestItemResult(x.Name, x.Weight)).ToList(),
            reply.PersonaLabel,
            reply.Summary);
    }

    public void Dispose() => _channel.Dispose();
}
