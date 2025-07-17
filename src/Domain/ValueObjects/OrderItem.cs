namespace Talabeyah.OrderManagement.Domain.ValueObjects;

public class OrderItem
{
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }

    public OrderItem(int productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }
} 