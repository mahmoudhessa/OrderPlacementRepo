namespace Talabeyah.OrderManagement.Application.Contracts;

public class PlaceOrderRequest
{
    public string? BuyerId { get; set; }
    public List<OrderProductDto> Products { get; set; } = new();
}

public class OrderProductDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
} 