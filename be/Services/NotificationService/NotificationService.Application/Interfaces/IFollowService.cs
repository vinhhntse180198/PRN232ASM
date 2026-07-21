using NotificationService.Application.DTOs;

namespace NotificationService.Application.Interfaces;

public interface IFollowService
{
    Task FollowTopicAsync(FollowTopicRequest request, CancellationToken cancellationToken = default);
    Task FollowKeywordAsync(FollowKeywordRequest request, CancellationToken cancellationToken = default);
    Task UnfollowTopicAsync(Guid userId, Guid topicId, CancellationToken cancellationToken = default);
    Task<FollowsSummaryDto> GetFollowsAsync(Guid userId, CancellationToken cancellationToken = default);
}
