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
        if (request.Roles.Contains("Admin"))
        {
            List<Domain.Entities.Order> orders;
            int total;
            orders = await _orderRepository.GetPagedAsync(request.Page, request.PageSize);
            total = await _orderRepository.CountAsync();
            var dtos = orders.Select(o => new OrderListItemDto(o.Id, o.Status.ToString(), o.CreatedAt)).ToList();
            return new GetOrdersResult(dtos, total);
        }
        else if (request.Roles.Contains("Buyer") && request.UserId != null)
        {
            List<Domain.Entities.Order> orders;
            int total;
            orders = await _orderRepository.GetPagedByBuyerAsync(request.UserId, request.Page, request.PageSize);
            total = await _orderRepository.CountByBuyerAsync(request.UserId);
            var dtos = orders.Select(o => new OrderListItemDto(o.Id, o.Status.ToString(), o.CreatedAt)).ToList();
            return new GetOrdersResult(dtos, total);
        }
        else
        {
            throw new UnauthorizedAccessException("User does not have permission to view these orders.");
        }
    }
} 