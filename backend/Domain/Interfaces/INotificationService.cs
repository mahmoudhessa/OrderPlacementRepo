namespace Talabeyah.OrderManagement.Domain.Interfaces;

public interface INotificationService
{
    Task NotifyOrderCreatedAsync(int orderId, DateTime createdAt, string status, CancellationToken cancellationToken = default);
    Task NotifyOrderStatusChangedAsync(int orderId, string oldStatus, string newStatus, string updatedBy, CancellationToken cancellationToken = default);
    Task NotifyInventoryChangedAsync(int productId, string productName, int oldQuantity, int newQuantity, string reason, CancellationToken cancellationToken = default);
    Task NotifyLowInventoryAsync(int productId, string productName, int currentQuantity, int threshold, CancellationToken cancellationToken = default);
    Task NotifyOrderCancelledAsync(int orderId, string reason, string cancelledBy, CancellationToken cancellationToken = default);
    Task NotifySystemAlertAsync(string alertType, string message, string severity = "Info", CancellationToken cancellationToken = default);
    Task NotifyConcurrencyConflictAsync(int orderId, string conflictType, CancellationToken cancellationToken = default);
} 