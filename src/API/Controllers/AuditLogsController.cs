using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Talabeyah.OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Auditor")]
public class AuditLogsController : ControllerBase
{
    [HttpGet]
    public IActionResult List()
    {
        // Placeholder: will implement GetAuditLogsQuery in Application layer
        return Ok();
    }
} 