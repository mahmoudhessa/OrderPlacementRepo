using Microsoft.AspNetCore.Mvc;

namespace Talabeyah.OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditLogsController : ControllerBase
{
    [HttpGet]
    public IActionResult List()
    {
        // Placeholder: will implement GetAuditLogsQuery in Application layer
        return Ok();
    }
} 