using Talabeyah.OrderManagement.Domain.Enums;
using Talabeyah.OrderManagement.Domain.ValueObjects;

namespace Talabeyah.OrderManagement.Domain.Entities;

public class Order
{
    public int Id { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public List<OrderItem> Items { get; private set; }

    public Order()
    {
        Items = new List<OrderItem>();
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddItem(int productId, int quantity)
    {
        Items.Add(new OrderItem(productId, quantity));
    }

    public void MarkAsCancelled()
    {
        Status = OrderStatus.Cancelled;
    }

    public void MarkAsCompleted()
    {
        Status = OrderStatus.Completed;
    }
} 