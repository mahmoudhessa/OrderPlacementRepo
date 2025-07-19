namespace Talabeyah.OrderManagement.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Inventory { get; set; }
    public byte[]? RowVersion { get; set; } // For optimistic concurrency
    public bool IsDeleted { get; set; } // For soft-delete
    public DateTime? PromotionExpiry { get; set; } // For promotional product expiry

    public Product(int id, string name, int inventory)
    {
        Id = id;
        Name = name;
        Inventory = inventory;
    }

    public Product() {}
} 