using MediatR;
using Talabeyah.OrderManagement.Domain.Entities;
using Talabeyah.OrderManagement.Domain.Interfaces;
using Talabeyah.OrderManagement.Domain.Services;
using Talabeyah.OrderManagement.Domain.Enums;

namespace Talabeyah.OrderManagement.Application.Orders.Commands;

public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly OrderDomainService _orderDomainService;
    private readonly ProductDomainService _productDomainService;

    public PlaceOrderCommandHandler(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        OrderDomainService orderDomainService,
        ProductDomainService productDomainService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _orderDomainService = orderDomainService;
        _productDomainService = productDomainService;
    }

    public async Task<int> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        // Business rule: max allowed quantity per order
        if (request.Products.Sum(p => p.Quantity) > 10)
            throw new Exception("Cannot order more than 10 items per order.");

        // Begin transaction for atomicity and concurrency control
        using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var order = new Order
            {
                BuyerId = request.BuyerId,
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