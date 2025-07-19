using Talabeyah.OrderManagement.Domain.Enums;
using Talabeyah.OrderManagement.Domain.ValueObjects;

namespace Talabeyah.OrderManagement.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public string BuyerId { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[]? RowVersion { get; set; } // For optimistic concurrency
    public List<OrderItem> Items { get; set; } = new();

    public Order()
    {
        Items = new List<OrderItem>();
    }
} 