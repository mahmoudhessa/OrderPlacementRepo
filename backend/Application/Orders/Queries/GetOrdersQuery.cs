using MediatR;

namespace Talabeyah.OrderManagement.Application.Orders.Queries;

public record GetOrdersQuery(string? BuyerId, int Page, int PageSize) : IRequest<GetOrdersResult>;

public record GetOrdersResult(List<OrderListItemDto> Orders, int TotalCount);

public record OrderListItemDto(int Id, string Status, DateTime CreatedAt); 