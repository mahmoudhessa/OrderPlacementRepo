using MediatR;
using Talabeyah.OrderManagement.Domain.Interfaces;
using Talabeyah.OrderManagement.Application.Contracts;

namespace Talabeyah.OrderManagement.Application.Orders.Queries;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, GetOrdersResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextAccessor _userContextAccessor;

    public GetOrdersQueryHandler(IUnitOfWork unitOfWork, IUserContextAccessor userContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _userContextAccessor = userContextAccessor;
    }

    public async Task<GetOrdersResult> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var userContext = _userContextAccessor.GetUserContext();
        if (userContext == null || string.IsNullOrEmpty(userContext.UserId))
            throw new UnauthorizedAccessException("User context is missing or invalid.");

        bool isAdmin = userContext.Roles.Contains("Admin");
        string? buyerId = request.BuyerId;
        if (!isAdmin)
        {
            // If not admin, restrict to own orders or specified buyerId if Sales
            if (userContext.Roles.Contains("Sales") && !string.IsNullOrEmpty(buyerId))
            {
                // Sales can view orders for any buyer
            }
            else
            {
                // Buyers can only view their own orders
                buyerId = userContext.UserId;
            }
        }
        // Fetch orders with filtering
        var orders = await _unitOfWork.OrderRepository.GetOrdersAsync(buyerId, request.Page, request.PageSize);
        int total = await _unitOfWork.OrderRepository.CountAsync();
        var dtos = orders.Select(o => new OrderListItemDto(o.Id, o.Status.ToString(), o.CreatedAt)).ToList();
        return new GetOrdersResult(dtos, total);
    }
} 