namespace Talabeyah.OrderManagement.Domain.Entities;

public class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int Inventory { get; private set; }

    public Product(int id, string name, int inventory)
    {
        Id = id;
        Name = name;
        Inventory = inventory;
    }

    public void DecreaseInventory(int quantity)
    {
        if (quantity > Inventory)
            throw new InvalidOperationException("Insufficient inventory.");
        Inventory -= quantity;
    }

    public void IncreaseInventory(int quantity)
    {
        Inventory += quantity;
    }
} 