using MediatR;
using Talabeyah.OrderManagement.Domain.Entities;
using Talabeyah.OrderManagement.Domain.Interfaces;
using Talabeyah.OrderManagement.Domain.Services;
using Talabeyah.OrderManagement.Domain.Enums;
using Talabeyah.OrderManagement.Application.Contracts;

namespace Talabeyah.OrderManagement.Application.Orders.Commands;

public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly OrderDomainService _orderDomainService;
    private readonly ProductDomainService _productDomainService;
    private readonly IUserContextAccessor _userContextAccessor;

    public PlaceOrderCommandHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        OrderDomainService orderDomainService,
        ProductDomainService productDomainService,
        IUserContextAccessor userContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _orderDomainService = orderDomainService;
        _productDomainService = productDomainService;
        _userContextAccessor = userContextAccessor;
    }

    public async Task<int> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var userContext = _userContextAccessor.GetUserContext();
        if (userContext == null || string.IsNullOrEmpty(userContext.UserId))
            throw new UnauthorizedAccessException("User context is missing or invalid.");

        string buyerId;
        if (userContext.Roles.Contains("Admin") || userContext.Roles.Contains("Sales"))
        {
            buyerId = request.BuyerId ?? userContext.UserId;
        }
        else if (userContext.Roles.Contains("Buyer"))
        {
            buyerId = userContext.UserId;
        }
        else
        {
            throw new UnauthorizedAccessException("User does not have permission to place orders.");
        }

        // Only Buyer can place orders for themselves
        // (If you want to restrict to buyers, check userContext.Roles and userContext.UserId here)

        // Business rule: max allowed quantity per order
        if (request.Products.Sum(p => p.Quantity) > 10)
            throw new Exception("Cannot order more than 10 items per order.");

        // Begin transaction for atomicity and concurrency control
        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var order = new Order
            {
                BuyerId = buyerId,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            
            // Validate all products and reserve inventory within transaction
            foreach (var item in request.Products)
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new Exception($"Product {item.ProductId} not found");
                if (product.IsDeleted)
                    throw new Exception($"Product {product.Name} is not available (deleted)");
                if (product.PromotionExpiry.HasValue && product.PromotionExpiry.Value < DateTime.UtcNow)
                    throw new Exception($"Product {product.Name} promotion has expired");
                if (item.Quantity > product.Inventory)
                    throw new Exception($"Insufficient inventory for product {product.Name}");
                
                // Decrease inventory and add to order within transaction
                _productDomainService.DecreaseInventory(product, item.Quantity);
                await _unitOfWork.ProductRepository.UpdateAsync(product);
                _orderDomainService.AddItemToOrder(order, item.ProductId, item.Quantity);
            }
            
            // Save order within the same transaction
            await _unitOfWork.OrderRepository.AddAsync(order);
            
            // Save all changes and commit transaction
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            
            // Notify after successful commit (outside transaction)
            await _notificationService.NotifyOrderCreatedAsync(order.Id, order.CreatedAt, order.Status.ToString(), cancellationToken);
            
            // Check for low inventory and notify if needed
            foreach (var item in request.Products)
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId);
                if (product != null && product.Inventory <= 10) // Low inventory threshold
                {
                    await _notificationService.NotifyLowInventoryAsync(
                        product.Id, 
                        product.Name, 
                        product.Inventory, 
                        10, 
                        cancellationToken);
                }
            }
            
            return order.Id;
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException ex)
        {
            // Rollback transaction on concurrency conflict
            await transaction.RollbackAsync(cancellationToken);
            throw new Talabeyah.OrderManagement.Domain.Exceptions.ConcurrencyException(
                "Order was modified by another user. Please try again.", ex);
        }
        catch (Exception)
        {
            // Rollback transaction on any other error
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
} 