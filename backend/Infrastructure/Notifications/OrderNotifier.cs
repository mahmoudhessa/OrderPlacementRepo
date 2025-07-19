/// <summary>
/// OrderNotifier is the single implementation of INotificationService for the system.
/// It handles all notification logic (Kafka, SignalR, etc) and is registered as INotificationService in DI.
/// </summary>
using Talabeyah.OrderManagement.Domain.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Talabeyah.OrderManagement.Domain.Enums;

namespace Talabeyah.OrderManagement.Infrastructure.Notifications;

public class OrderNotifier : INotificationService
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<OrderNotifier> _logger;
    private readonly ISignalRService _signalRService;
    private const string OrderCreatedTopic = "order-created";

    public OrderNotifier(
        IProducer<string, string> producer, 
        ILogger<OrderNotifier> logger,
        ISignalRService signalRService)
    {
        _producer = producer;
        _logger = logger;
        _signalRService = signalRService;
    }

    public async Task NotifyOrderCreatedAsync(int orderId, DateTime createdAt, string status, CancellationToken cancellationToken = default)
    {
        try
        {
            // Send to Kafka for audit processing
            var kafkaMessage = new Message<string, string>
            {
                Key = orderId.ToString(),
                Value = $"{{\"orderId\":{orderId},\"createdAt\":\"{createdAt:O}\",\"status\":\"{status}\"}}"
            };

            var result = await _producer.ProduceAsync(OrderCreatedTopic, kafkaMessage, cancellationToken);
            _logger.LogInformation("Order created notification sent to Kafka: OrderId={OrderId}, Topic={Topic}, Partition={Partition}, Offset={Offset}", 
                orderId, result.Topic, result.Partition, result.Offset);

            // Send real-time notification via SignalR
            var signalRMessage = new
            {
                orderId,
                createdAt,
                status,
                eventType = "OrderCreated",
                message = $"New order #{orderId} created with status: {status}"
            };

            // Notify all users with Sales and Admin roles
            await _signalRService.SendToGroupsAsync(new[] { "Sales", "Admin" }, "OrderCreated", signalRMessage, cancellationToken);
            _logger.LogInformation("Order created notification sent via SignalR: OrderId={OrderId}", orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send order created notification for OrderId={OrderId}", orderId);
            throw;
        }
    }

    public async Task NotifyOrderStatusChangedAsync(int orderId, string oldStatus, string newStatus, string updatedBy, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new
            {
                orderId,
                oldStatus,
                newStatus,
                updatedBy,
                updatedAt = DateTime.UtcNow,
                eventType = "OrderStatusChanged",
                message = $"Order #{orderId} status changed from {oldStatus} to {newStatus} by {updatedBy}"
            };

            // Notify specific order group and role-based groups
            await _signalRService.SendToGroupsAsync(new[] { $"Order_{orderId}", "Sales", "Admin" }, "OrderStatusChanged", message, cancellationToken);
            _logger.LogInformation("Order status change notification sent: OrderId={OrderId}, Status: {OldStatus} -> {NewStatus}", orderId, oldStatus, newStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send order status change notification for OrderId={OrderId}", orderId);
            throw;
        }
    }

    public async Task NotifyInventoryChangedAsync(int productId, string productName, int oldQuantity, int newQuantity, string reason, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new
            {
                productId,
                productName,
                oldQuantity,
                newQuantity,
                change = newQuantity - oldQuantity,
                reason,
                updatedAt = DateTime.UtcNow,
                eventType = "InventoryChanged",
                message = $"Product '{productName}' inventory changed from {oldQuantity} to {newQuantity} ({reason})"
            };

            // Notify inventory managers and admins
            await _signalRService.SendToGroupsAsync(new[] { "InventoryManager", "Admin" }, "InventoryChanged", message, cancellationToken);
            _logger.LogInformation("Inventory change notification sent: ProductId={ProductId}, Quantity: {OldQuantity} -> {NewQuantity}", productId, oldQuantity, newQuantity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send inventory change notification for ProductId={ProductId}", productId);
            throw;
        }
    }

    public async Task NotifyLowInventoryAsync(int productId, string productName, int currentQuantity, int threshold, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new
            {
                productId,
                productName,
                currentQuantity,
                threshold,
                updatedAt = DateTime.UtcNow,
                eventType = "LowInventory",
                message = $"Low inventory alert: Product '{productName}' has {currentQuantity} items remaining (threshold: {threshold})"
            };

            // Notify inventory managers and admins
            await _signalRService.SendToGroupsAsync(new[] { "InventoryManager", "Admin" }, "LowInventory", message, cancellationToken);
            _logger.LogInformation("Low inventory notification sent: ProductId={ProductId}, Quantity={Quantity}, Threshold={Threshold}", productId, currentQuantity, threshold);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send low inventory notification for ProductId={ProductId}", productId);
            throw;
        }
    }

    public async Task NotifyOrderCancelledAsync(int orderId, string reason, string cancelledBy, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new
            {
                orderId,
                reason,
                cancelledBy,
                cancelledAt = DateTime.UtcNow,
                eventType = "OrderCancelled",
                message = $"Order #{orderId} was cancelled by {cancelledBy}. Reason: {reason}"
            };

            // Notify all relevant groups
            await _signalRService.SendToGroupsAsync(new[] { $"Order_{orderId}", "Sales", "Admin" }, "OrderCancelled", message, cancellationToken);
            _logger.LogInformation("Order cancelled notification sent: OrderId={OrderId}, Reason={Reason}", orderId, reason);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send order cancelled notification for OrderId={OrderId}", orderId);
            throw;
        }
    }

    public async Task NotifySystemAlertAsync(string alertType, string message, string severity = "Info", CancellationToken cancellationToken = default)
    {
        try
        {
            var alert = new
            {
                alertType,
                message,
                severity,
                timestamp = DateTime.UtcNow,
                eventType = "SystemAlert"
            };

            // Notify all users in system alerts group
            await _signalRService.SendToGroupsAsync(new[] { "SystemAlerts", "Admin" }, "SystemAlert", alert, cancellationToken);
            _logger.LogInformation("System alert notification sent: Type={AlertType}, Severity={Severity}", alertType, severity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send system alert notification: Type={AlertType}", alertType);
            throw;
        }
    }

    public async Task NotifyConcurrencyConflictAsync(int orderId, string conflictType, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new
            {
                orderId,
                conflictType,
                timestamp = DateTime.UtcNow,
                eventType = "ConcurrencyConflict",
                message = $"Concurrency conflict detected for order #{orderId}: {conflictType}"
            };

            // Notify admins and sales about concurrency conflicts
            await _signalRService.SendToGroupsAsync(new[] { "Admin", "Sales" }, "ConcurrencyConflict", message, cancellationToken);
            _logger.LogInformation("Concurrency conflict notification sent: OrderId={OrderId}, Type={ConflictType}", orderId, conflictType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send concurrency conflict notification for OrderId={OrderId}", orderId);
            throw;
        }
    }
} 