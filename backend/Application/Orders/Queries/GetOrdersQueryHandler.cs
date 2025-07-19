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
        List<Domain.Entities.Order> orders;
        int total;
        if (string.IsNullOrEmpty(request.BuyerId))
        {
            orders = await _orderRepository.GetPagedAsync(request.Page, request.PageSize);
            total = await _orderRepository.CountAsync();
        }
        else
        {
            orders = await _orderRepository.GetPagedByBuyerAsync(request.BuyerId, request.Page, request.PageSize);
            total = await _orderRepository.CountByBuyerAsync(request.BuyerId);
        }
        var dtos = orders.Select(o => new OrderListItemDto(o.Id, o.Status.ToString(), o.CreatedAt)).ToList();
        return new GetOrdersResult(dtos, total);
    }
} 