using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;

namespace NotificationService.Application.Services;

public class NotificationAppService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    public NotificationAppService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IReadOnlyList<NotificationResponse>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var items = await _unitOfWork.Notifications.GetByUserIdAsync(userId, cancellationToken);
        return items.Select(Map).ToList();
    }

    public async Task<NotificationResponse> CreateAsync(CreateNotificationRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new InvalidOperationException("Title is required.");

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            PaperId = request.PaperId,
            Type = request.Type,
            Title = request.Title.Trim(),
            Message = request.Message.Trim(),
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Notifications.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(notification);
    }

    public async Task MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _ = await _unitOfWork.Notifications.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Notification not found.");
        await _unitOfWork.Notifications.MarkAsReadAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static NotificationResponse Map(Notification n) => new()
    {
        Id = n.Id, UserId = n.UserId, PaperId = n.PaperId, Type = n.Type,
        Title = n.Title, Message = n.Message, IsRead = n.IsRead, CreatedAt = n.CreatedAt
    };
}
