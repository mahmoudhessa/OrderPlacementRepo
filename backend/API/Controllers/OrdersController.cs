using MediatR;
using Microsoft.AspNetCore.Mvc;
using Talabeyah.OrderManagement.Application.Orders.Commands;
using Talabeyah.OrderManagement.Application.Orders.Queries;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Talabeyah.OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidator<PlaceOrderCommand> _validator;
    public OrdersController(IMediator mediator, IValidator<PlaceOrderCommand> validator)
    {
        _mediator = mediator;
        _validator = validator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PlaceOrderCommand clientCommand)
    {
        var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        var roles = User.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();
        clientCommand.UserId = userId;
        clientCommand.Roles = roles;
        try
        {
            var orderId = await _mediator.Send(clientCommand);
            return Ok(new { orderId });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        var roles = User.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();
        var query = new GetOrdersQuery(null, page, pageSize) { UserId = userId, Roles = roles };
        try
        {
            var orders = await _mediator.Send(query);
            return Ok(orders);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
} 