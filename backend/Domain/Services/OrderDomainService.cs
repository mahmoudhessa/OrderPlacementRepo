using Talabeyah.OrderManagement.Domain.Entities;
using Talabeyah.OrderManagement.Domain.Enums;
using Talabeyah.OrderManagement.Domain.ValueObjects;

namespace Talabeyah.OrderManagement.Domain.Services;

public class OrderDomainService
{
    public void AddItemToOrder(Order order, int productId, int quantity)
    {
        // Business rules
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");
            
        if (order.Items.Count >= 10)
            throw new InvalidOperationException("Order cannot have more than 10 items");
            
        var item = new OrderItem(productId, quantity);
        order.Items.Add(item);
        order.UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateOrderStatus(Order order, OrderStatus newStatus)
    {
        // Business rules
        if (order.Status == OrderStatus.Completed && newStatus == OrderStatus.Pending)
            throw new InvalidOperationException("Cannot revert completed order to pending");
            
        order.Status = newStatus;
        order.UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsCancelled(Order order)
    {
        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsCompleted(Order order)
    {
        order.Status = OrderStatus.Completed;
        order.UpdatedAt = DateTime.UtcNow;
    }
} 