using MediatR;

namespace Talabeyah.OrderManagement.Application.Orders.Commands;

public record PlaceOrderCommand(string BuyerId, List<OrderProductDto> Products) : IRequest<int>
{
    public string? UserId { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
}

public record OrderProductDto(int ProductId, int Quantity); 