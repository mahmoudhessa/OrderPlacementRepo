namespace Talabeyah.OrderManagement.Application.Orders.Commands;

public interface IOrderNotifier
{
    Task NotifyOrderCreatedAsync(int orderId, DateTime createdAt, string status, CancellationToken cancellationToken = default);
} 