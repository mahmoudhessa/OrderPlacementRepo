using Talabeyah.OrderManagement.Domain.Entities;

namespace Talabeyah.OrderManagement.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);
    Task<List<Order>> GetOrdersAsync(string? buyerId, int page, int pageSize);
    Task<int> CountAsync(string? buyerId = null);
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
} 