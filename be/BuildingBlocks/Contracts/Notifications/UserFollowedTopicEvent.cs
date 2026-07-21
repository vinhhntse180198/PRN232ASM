using PRN232ASM.BuildingBlocks.Contracts.Abstractions;

namespace PRN232ASM.BuildingBlocks.Contracts.Notifications;

public class UserFollowedTopicEvent : IntegrationEvent
{
    public Guid UserId { get; set; }
    public Guid TopicId { get; set; }
}
