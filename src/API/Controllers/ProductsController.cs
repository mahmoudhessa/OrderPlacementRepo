using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Talabeyah.OrderManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    public ProductsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> List()
    {
        // Placeholder: will implement GetProductsQuery in Application layer
        return Ok();
    }
} 