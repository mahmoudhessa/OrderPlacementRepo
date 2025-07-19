using Talabeyah.OrderManagement.Domain.Entities;

namespace Talabeyah.OrderManagement.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);
    Task<List<Order>> GetPagedAsync(int page, int pageSize);
    Task<List<Order>> GetPagedByBuyerAsync(string buyerId, int page, int pageSize);
    Task<int> CountAsync();
    Task<int> CountByBuyerAsync(string buyerId);
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
} 