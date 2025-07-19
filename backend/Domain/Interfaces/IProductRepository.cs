using Talabeyah.OrderManagement.Domain.Entities;

namespace Talabeyah.OrderManagement.Domain.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<List<Product>> GetAllAsync();
    Task UpdateAsync(Product product);
    Task AddAsync(Product product);
} 