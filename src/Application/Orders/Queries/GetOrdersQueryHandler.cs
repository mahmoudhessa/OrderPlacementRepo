using MediatR;
using Talabeyah.OrderManagement.Domain.Interfaces;

namespace Talabeyah.OrderManagement.Application.Orders.Queries;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, GetOrdersResult>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<GetOrdersResult> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetPagedAsync(request.Page, request.PageSize);
        var total = await _orderRepository.CountAsync();
        var dtos = orders.Select(o => new OrderListItemDto(o.Id, o.Status.ToString(), o.CreatedAt)).ToList();
        return new GetOrdersResult(dtos, total);
    }
} 