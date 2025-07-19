using MediatR;

namespace Talabeyah.OrderManagement.Application.Orders.Queries;

public record GetOrdersQuery(string? BuyerId, int Page, int PageSize) : IRequest<GetOrdersResult>
{
    public string? UserId { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
}

public record GetOrdersResult(List<OrderListItemDto> Orders, int TotalCount);

public record OrderListItemDto(int Id, string Status, DateTime CreatedAt); 