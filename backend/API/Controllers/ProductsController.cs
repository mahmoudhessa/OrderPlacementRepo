using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Talabeyah.OrderManagement.Application.Products.Queries;

namespace Talabeyah.OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize(Roles = "Admin,Sales,InventoryManager")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    public ProductsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? name = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var products = await _mediator.Send(new GetProductsQuery(name, page, pageSize));
        return Ok(products);
    }
} 