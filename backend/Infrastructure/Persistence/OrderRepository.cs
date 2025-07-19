using Microsoft.EntityFrameworkCore;
using Talabeyah.OrderManagement.Domain.Entities;
using Talabeyah.OrderManagement.Domain.Interfaces;

namespace Talabeyah.OrderManagement.Infrastructure.Persistence;

public class OrderRepository : IOrderRepository
{
    private readonly OrderManagementDbContext _context;

    public OrderRepository(OrderManagementDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<List<Order>> GetPagedAsync(int page, int pageSize)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<Order>> GetPagedByBuyerAsync(string buyerId, int page, int pageSize)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.BuyerId == buyerId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync()
    {
        return await _context.Orders.CountAsync();
    }

    public async Task<int> CountByBuyerAsync(string buyerId)
    {
        return await _context.Orders.CountAsync(o => o.BuyerId == buyerId);
    }

    public async Task AddAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }
} 