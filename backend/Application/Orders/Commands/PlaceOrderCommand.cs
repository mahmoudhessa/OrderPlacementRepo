using MediatR;
using System.Collections.Generic;
using Talabeyah.OrderManagement.Application.Contracts;

namespace Talabeyah.OrderManagement.Application.Orders.Commands;

public record PlaceOrderCommand(string? BuyerId, List<OrderProductDto> Products) : IRequest<int>
{
} 