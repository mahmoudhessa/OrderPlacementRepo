using Talabeyah.OrderManagement.Domain.Entities;

namespace Talabeyah.OrderManagement.Domain.Services;

public class ProductDomainService
{
    public void DecreaseInventory(Product product, int quantity)
    {
        if (quantity > product.Inventory)
            throw new InvalidOperationException("Insufficient inventory.");
        product.Inventory -= quantity;
    }

    public void IncreaseInventory(Product product, int quantity)
    {
        product.Inventory += quantity;
    }
} 