using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Talabeyah.OrderManagement.Application.Audit.Queries;

namespace Talabeyah.OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AuditLogsController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuditLogsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? change = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetAuditLogsQuery(change, page, pageSize);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
} 