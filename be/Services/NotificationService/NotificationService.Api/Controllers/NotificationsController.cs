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

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<Application.DTOs.NotificationDto>>>> GetByUser(
        [FromQuery] Guid userId,
        CancellationToken cancellationToken)
    {
        var data = await _notificationService.GetByUserIdAsync(userId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<Application.DTOs.NotificationDto>>.Ok(data));
    }

    [HttpPut("{id:guid}/read")]
    public async Task<ActionResult<ApiResponse<object>>> MarkAsRead(Guid id, CancellationToken cancellationToken)
    {
        await _notificationService.MarkAsReadAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Notification marked as read."));
    }

    [HttpPut("read-all")]
    public async Task<ActionResult<ApiResponse<object>>> MarkAllAsRead(
        [FromQuery] Guid userId,
        CancellationToken cancellationToken)
    {
        await _notificationService.MarkAllAsReadAsync(userId, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "All notifications marked as read."));
    }
}
