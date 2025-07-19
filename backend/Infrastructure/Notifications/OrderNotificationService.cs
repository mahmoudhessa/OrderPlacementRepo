using Microsoft.Extensions.Logging;
using Talabeyah.OrderManagement.Domain.Interfaces;

namespace Talabeyah.OrderManagement.Infrastructure.Notifications;

public class OrderNotificationService : INotificationService
{
    private readonly ISignalRService _signalRService;
    private readonly ILogger<OrderNotificationService> _logger;

    public OrderNotificationService(ISignalRService signalRService, ILogger<OrderNotificationService> logger)
    {
        _signalRService = signalRService;
        _logger = logger;
    }

    public async Task NotifyOrderCreatedAsync(int orderId, DateTime createdAt, string status, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new
            {
                type = "OrderCreated",
                orderId,
                createdAt,
                status,
                timestamp = DateTime.UtcNow
            };

            await _signalRService.SendToGroupAsync("orders", "OrderCreated", message, cancellationToken);
            _logger.LogInformation("Order created notification sent for order {OrderId}", orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send order created notification for order {OrderId}", orderId);
            throw;
        }
    }

    public async Task NotifyOrderStatusChangedAsync(int orderId, string oldStatus, string newStatus, string updatedBy, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new
            {
                type = "OrderStatusChanged",
                orderId,
                oldStatus,
                newStatus,
                updatedBy,
                timestamp = DateTime.UtcNow
            };

            await _signalRService.SendToGroupAsync("orders", "OrderStatusChanged", message, cancellationToken);
            _logger.LogInformation("Order status changed notification sent for order {OrderId}: {OldStatus} -> {NewStatus}", orderId, oldStatus, newStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send order status changed notification for order {OrderId}", orderId);
            throw;
        }
    }

    public async Task NotifyInventoryChangedAsync(int productId, string productName, int oldQuantity, int newQuantity, string reason, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new
            {
                type = "InventoryChanged",
                productId,
                productName,
                oldQuantity,
                newQuantity,
                reason,
                timestamp = DateTime.UtcNow
            };

            await _signalRService.SendToGroupAsync("inventory", "InventoryChanged", message, cancellationToken);
            _logger.LogInformation("Inventory changed notification sent for product {ProductId}: {OldQuantity} -> {NewQuantity}", productId, oldQuantity, newQuantity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send inventory changed notification for product {ProductId}", productId);
            throw;
        }
    }

    public async Task NotifyLowInventoryAsync(int productId, string productName, int currentQuantity, int threshold, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new
            {
                type = "LowInventory",
                productId,
                productName,
                currentQuantity,
                threshold,
                timestamp = DateTime.UtcNow
            };

            await _signalRService.SendToGroupAsync("inventory", "LowInventory", message, cancellationToken);
            _logger.LogWarning("Low inventory notification sent for product {ProductId}: {CurrentQuantity} < {Threshold}", productId, currentQuantity, threshold);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send low inventory notification for product {ProductId}", productId);
            throw;
        }
    }

    public async Task NotifyOrderCancelledAsync(int orderId, string reason, string cancelledBy, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new
            {
                type = "OrderCancelled",
                orderId,
                reason,
                cancelledBy,
                timestamp = DateTime.UtcNow
            };

            await _signalRService.SendToGroupAsync("orders", "OrderCancelled", message, cancellationToken);
            _logger.LogInformation("Order cancelled notification sent for order {OrderId}", orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send order cancelled notification for order {OrderId}", orderId);
            throw;
        }
    }

    public async Task NotifySystemAlertAsync(string alertType, string message, string severity = "Info", CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = new
            {
                type = "SystemAlert",
                alertType,
                message,
                severity,
                timestamp = DateTime.UtcNow
            };

            await _signalRService.SendToGroupAsync("system", "SystemAlert", notification, cancellationToken);
            _logger.LogInformation("System alert notification sent: {AlertType} - {Message}", alertType, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send system alert notification: {AlertType}", alertType);
            throw;
        }
    }

    public async Task NotifyConcurrencyConflictAsync(int orderId, string conflictType, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new
            {
                type = "ConcurrencyConflict",
                orderId,
                conflictType,
                timestamp = DateTime.UtcNow
            };

            await _signalRService.SendToGroupAsync("orders", "ConcurrencyConflict", message, cancellationToken);
            _logger.LogWarning("Concurrency conflict notification sent for order {OrderId}: {ConflictType}", orderId, conflictType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send concurrency conflict notification for order {OrderId}", orderId);
            throw;
        }
    }
} 