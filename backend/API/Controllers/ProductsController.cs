using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Talabeyah.OrderManagement.Application.Products.Queries;
using Talabeyah.OrderManagement.Application.Products.Commands;
using Talabeyah.OrderManagement.Application.Contracts;
using System.Security.Claims;

namespace Talabeyah.OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,InventoryManager,Buyer")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    public ProductsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? name = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var query = new GetProductsQuery(name, page, pageSize);
        try
        {
            var products = await _mediator.Send(query);
            return Ok(products);
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

    [HttpPost]
    [Authorize(Roles = "Admin,InventoryManager")]
    public async Task<IActionResult> Add([FromBody] AddProductRequest request)
    {
        var command = new AddProductCommand(request.Name, request.Inventory);
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
} 