using NotificationService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using PRN232ASM.BuildingBlocks.Common.Models;

namespace NotificationService.Api.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    private Guid GetUserId()
    {
        if (Request.Headers.TryGetValue("X-User-Id", out var headerValue) &&
            Guid.TryParse(headerValue.FirstOrDefault(), out var userId) &&
            userId != Guid.Empty)
        {
            return userId;
        }
        throw new UnauthorizedAccessException("X-User-Id header is required.");
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<Application.DTOs.NotificationDto>>>> GetByUser(
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var data = await _notificationService.GetByUserIdAsync(userId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<Application.DTOs.NotificationDto>>.Ok(data));
    }

    [HttpPut("{id:guid}/read")]
    public async Task<ActionResult<ApiResponse<object>>> MarkAsRead(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        await _notificationService.MarkAsReadAsync(id, userId, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Notification marked as read."));
    }

    [HttpPut("read-all")]
    public async Task<ActionResult<ApiResponse<object>>> MarkAllAsRead(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        await _notificationService.MarkAllAsReadAsync(userId, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "All notifications marked as read."));
    }
}
