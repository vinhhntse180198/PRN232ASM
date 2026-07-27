using AutoMapper;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using PRN232ASM.BuildingBlocks.Common.Exceptions;

namespace NotificationService.Application.Services;

public class NotificationAppService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public NotificationAppService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<NotificationDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var items = await _unitOfWork.Notifications.GetByUserIdAsync(userId, cancellationToken);
        return _mapper.Map<IReadOnlyList<NotificationDto>>(items);
    }

    public async Task MarkAsReadAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Notification", id);

        if (notification.UserId != userId)
            throw new UnauthorizedAccessException("Cannot mark another user's notification as read.");

        notification.IsRead = true;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.Notifications.MarkAllAsReadAsync(userId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateAsync(Guid userId, string title, string message, string type, CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Notifications.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
