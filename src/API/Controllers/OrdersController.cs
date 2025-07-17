using MediatR;
using Microsoft.AspNetCore.Mvc;
using Talabeyah.OrderManagement.Application.Orders.Commands;
using Talabeyah.OrderManagement.Application.Orders.Queries;
using FluentValidation;
using FluentValidation.Results;

namespace Talabeyah.OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    public async Task<IActionResult> Create([FromBody] PlaceOrderCommand command)
    {
        ValidationResult validationResult = await _validator.ValidateAsync(command);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
        try
        {
            var orderId = await _mediator.Send(command);
            return Ok(new { orderId });
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("not found"))
                return NotFound(ex.Message);
            if (ex.Message.Contains("Insufficient inventory"))
                return Conflict(ex.Message);
            if (ex.Message.Contains("concurrency"))
                return Conflict(ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetOrdersQuery(page, pageSize));
        return Ok(result);
    }
} 