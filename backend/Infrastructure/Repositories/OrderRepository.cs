using Microsoft.EntityFrameworkCore;
using Talabeyah.OrderManagement.Domain.Entities;
using Talabeyah.OrderManagement.Domain.Interfaces;
using Talabeyah.OrderManagement.Infrastructure.Persistence;

namespace Talabeyah.OrderManagement.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderManagementDbContext _db;
    public OrderRepository(OrderManagementDbContext db) => _db = db;

    public async Task<Order?> GetByIdAsync(int id)
        => await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);

    public async Task<List<Order>> GetOrdersAsync(string? buyerId, int page, int pageSize)
    {
        var query = _db.Orders.Include(o => o.Items).AsQueryable();
        if (!string.IsNullOrEmpty(buyerId))
            query = query.Where(o => o.BuyerId == buyerId);
        return await query.OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync(string? buyerId = null)
    {
        if (!string.IsNullOrEmpty(buyerId))
            return await _db.Orders.CountAsync(o => o.BuyerId == buyerId);
        return await _db.Orders.CountAsync();
    }

    public async Task AddAsync(Order order)
    {
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        _db.Orders.Update(order);
        await _db.SaveChangesAsync();
    }
} 