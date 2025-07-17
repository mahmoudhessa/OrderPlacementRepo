using MediatR;

namespace Talabeyah.OrderManagement.Application.Orders.Commands;

public record PlaceOrderCommand(List<OrderProductDto> Products) : IRequest<int>;

public record OrderProductDto(int ProductId, int Quantity); 