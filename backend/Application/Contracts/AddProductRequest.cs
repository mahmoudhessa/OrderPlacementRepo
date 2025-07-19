namespace Talabeyah.OrderManagement.Application.Contracts;

public class AddProductRequest
{
    public string Name { get; set; } = string.Empty;
    public int Inventory { get; set; }
} 