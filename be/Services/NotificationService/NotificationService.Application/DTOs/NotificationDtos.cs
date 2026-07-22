namespace NotificationService.Application.DTOs;

public record NotificationDto(
    Guid Id,
    Guid UserId,
    string Title,
    string Message,
    bool IsRead,
    DateTime CreatedAt,
    string Type);

public record FollowTopicRequest(Guid UserId, Guid TopicId);
public record FollowKeywordRequest(Guid UserId, Guid KeywordId);

public record FollowsSummaryDto(
    IReadOnlyList<Guid> TopicIds,
    IReadOnlyList<Guid> KeywordIds,
    IReadOnlyList<Guid> JournalIds);
