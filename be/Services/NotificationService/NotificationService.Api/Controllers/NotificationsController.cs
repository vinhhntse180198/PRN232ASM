using NotificationService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace NotificationService.Api.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    public NotificationsController(INotificationService notificationService) => _notificationService = notificationService;

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetByUser(Guid userId, CancellationToken cancellationToken)
        => Ok(new { data = await _notificationService.GetByUserIdAsync(userId, cancellationToken) });

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Application.DTOs.CreateNotificationRequest request, CancellationToken cancellationToken)
    {
        try { return Ok(new { data = await _notificationService.CreateAsync(request, cancellationToken) }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken)
    {
        try { await _notificationService.MarkAsReadAsync(id, cancellationToken); return Ok(new { message = "Marked as read." }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }
}
