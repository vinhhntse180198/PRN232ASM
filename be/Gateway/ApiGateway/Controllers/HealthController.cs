using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public ActionResult<ApiResponse<object>> Get() =>
        Ok(ApiResponse<object>.Ok(new
        {
            service = "ApiGateway",
            status = "healthy",
            timestamp = DateTime.UtcNow
        }));
}
