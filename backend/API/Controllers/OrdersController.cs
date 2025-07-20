using MediatR;
using Microsoft.AspNetCore.Mvc;
using Talabeyah.OrderManagement.Application.Orders.Commands;
using Talabeyah.OrderManagement.Application.Orders.Queries;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Talabeyah.OrderManagement.Application.Contracts;

namespace Talabeyah.OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Buyer")]
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
    public async Task<IActionResult> Create([FromBody] PlaceOrderRequest clientCommand)
    {
        var command = new PlaceOrderCommand(clientCommand.BuyerId, clientCommand.Products);
        try
        {
            var orderId = await _mediator.Send(command);
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
    public async Task<IActionResult> List([FromQuery] string? buyerId = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetOrdersQuery(buyerId, page, pageSize);
        try
        {
            var orders = await _mediator.Send(query);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
} 