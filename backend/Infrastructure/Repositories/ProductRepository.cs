using Microsoft.EntityFrameworkCore;
using Talabeyah.OrderManagement.Domain.Entities;
using Talabeyah.OrderManagement.Domain.Interfaces;
using Talabeyah.OrderManagement.Infrastructure.Persistence;

namespace Talabeyah.OrderManagement.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly OrderManagementDbContext _db;
    public ProductRepository(OrderManagementDbContext db) => _db = db;

    public async Task<Product?> GetByIdAsync(int id)
        => await _db.Products.FirstOrDefaultAsync(p => p.Id == id);

    public async Task<List<Product>> GetAllAsync()
        => await _db.Products.ToListAsync();

    public async Task UpdateAsync(Product product)
    {
        _db.Products.Update(product);
        await _db.SaveChangesAsync();
    }

    public async Task AddAsync(Product product)
    {
        await _db.Products.AddAsync(product);
        await _db.SaveChangesAsync();
    }
} 