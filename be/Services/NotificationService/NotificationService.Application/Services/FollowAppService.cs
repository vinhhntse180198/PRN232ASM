using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using PRN232ASM.BuildingBlocks.Common.Exceptions;
using PRN232ASM.BuildingBlocks.Contracts.Notifications;
using PRN232ASM.BuildingBlocks.EventBus.Abstractions;

namespace NotificationService.Application.Services;

public class FollowAppService : IFollowService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventBus _eventBus;

    public FollowAppService(IUnitOfWork unitOfWork, IEventBus eventBus)
    {
        _unitOfWork = unitOfWork;
        _eventBus = eventBus;
    }

    public async Task FollowTopicAsync(FollowTopicRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _unitOfWork.Follows.GetTopicFollowAsync(request.UserId, request.TopicId, cancellationToken);
        if (existing is not null)
            throw new ValidationException("User already follows this topic.");

        await _unitOfWork.Follows.AddTopicFollowAsync(new FollowTopic
        {
            UserId = request.UserId,
            TopicId = request.TopicId,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _eventBus.PublishAsync(new UserFollowedTopicEvent
        {
            UserId = request.UserId,
            TopicId = request.TopicId
        }, cancellationToken);
    }

    public async Task FollowKeywordAsync(FollowKeywordRequest request, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.Follows.AddKeywordFollowAsync(new FollowKeyword
        {
            UserId = request.UserId,
            KeywordId = request.KeywordId,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UnfollowTopicAsync(Guid userId, Guid topicId, CancellationToken cancellationToken = default)
    {
        var existing = await _unitOfWork.Follows.GetTopicFollowAsync(userId, topicId, cancellationToken)
            ?? throw new NotFoundException("FollowTopic", $"{userId}/{topicId}");

        await _unitOfWork.Follows.RemoveTopicFollowAsync(userId, topicId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<FollowsSummaryDto> GetFollowsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var summary = await _unitOfWork.Follows.GetFollowsSummaryAsync(userId, cancellationToken);
        return new FollowsSummaryDto(summary.TopicIds, summary.KeywordIds, summary.JournalIds);
    }
}
