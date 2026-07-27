using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using PRN232ASM.BuildingBlocks.Common.Exceptions;
using PRN232ASM.BuildingBlocks.Contracts.Notifications;
using PRN232ASM.BuildingBlocks.EventBus.Outbox;

namespace NotificationService.Application.Services;

public class FollowAppService : IFollowService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxWriter _outbox;

    public FollowAppService(IUnitOfWork unitOfWork, IOutboxWriter outbox)
    {
        _unitOfWork = unitOfWork;
        _outbox = outbox;
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

        await _outbox.EnqueueAsync(new UserFollowedTopicEvent
        {
            UserId = request.UserId,
            TopicId = request.TopicId
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task FollowKeywordAsync(FollowKeywordRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _unitOfWork.Follows.GetKeywordFollowAsync(request.UserId, request.KeywordId, cancellationToken);
        if (existing is not null)
            throw new ValidationException("User already follows this keyword.");

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

    public async Task UnfollowKeywordAsync(Guid userId, Guid keywordId, CancellationToken cancellationToken = default)
    {
        var existing = await _unitOfWork.Follows.GetKeywordFollowAsync(userId, keywordId, cancellationToken)
            ?? throw new NotFoundException("FollowKeyword", $"{userId}/{keywordId}");

        await _unitOfWork.Follows.RemoveKeywordFollowAsync(userId, keywordId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task FollowJournalAsync(FollowJournalRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _unitOfWork.Follows.GetJournalFollowAsync(request.UserId, request.JournalId, cancellationToken);
        if (existing is not null)
            throw new ValidationException("User already follows this journal.");

        await _unitOfWork.Follows.AddJournalFollowAsync(new FollowJournal
        {
            UserId = request.UserId,
            JournalId = request.JournalId,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UnfollowJournalAsync(Guid userId, Guid journalId, CancellationToken cancellationToken = default)
    {
        var existing = await _unitOfWork.Follows.GetJournalFollowAsync(userId, journalId, cancellationToken)
            ?? throw new NotFoundException("FollowJournal", $"{userId}/{journalId}");

        await _unitOfWork.Follows.RemoveJournalFollowAsync(userId, journalId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<FollowsSummaryDto> GetFollowsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var summary = await _unitOfWork.Follows.GetFollowsSummaryAsync(userId, cancellationToken);
        return new FollowsSummaryDto(summary.TopicIds, summary.KeywordIds, summary.JournalIds);
    }
}
