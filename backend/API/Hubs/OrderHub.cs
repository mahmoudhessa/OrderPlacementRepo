using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace Talabeyah.OrderManagement.API.Hubs;

[Authorize]
public class OrderHub : Hub
{
    private readonly ILogger<OrderHub> _logger;

    public OrderHub(ILogger<OrderHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
        
        _logger.LogInformation("User {UserId} with role {Role} connected to SignalR", userId, userRole);
        
        // Add user to appropriate groups based on role
        if (!string.IsNullOrEmpty(userRole))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userRole);
            _logger.LogInformation("User {UserId} added to group {Group}", userId, userRole);
        }
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _logger.LogInformation("User {UserId} disconnected from SignalR", userId);
        
        await base.OnDisconnectedAsync(exception);
    }

    // Client-to-server methods for real-time interactions
    public async Task JoinOrderUpdates(int orderId)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Order_{orderId}");
        _logger.LogInformation("User {UserId} joined order updates for OrderId {OrderId}", userId, orderId);
    }

    public async Task LeaveOrderUpdates(int orderId)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Order_{orderId}");
        _logger.LogInformation("User {UserId} left order updates for OrderId {OrderId}", userId, orderId);
    }

    public async Task JoinInventoryUpdates()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await Groups.AddToGroupAsync(Context.ConnectionId, "InventoryUpdates");
        _logger.LogInformation("User {UserId} joined inventory updates", userId);
    }

    public async Task LeaveInventoryUpdates()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "InventoryUpdates");
        _logger.LogInformation("User {UserId} left inventory updates", userId);
    }

    public async Task JoinSystemAlerts()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await Groups.AddToGroupAsync(Context.ConnectionId, "SystemAlerts");
        _logger.LogInformation("User {UserId} joined system alerts", userId);
    }

    public async Task LeaveSystemAlerts()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SystemAlerts");
        _logger.LogInformation("User {UserId} left system alerts", userId);
    }

    // Method to get current connection info (for debugging)
    public async Task<string> GetConnectionInfo()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
        var connectionId = Context.ConnectionId;
        
        return $"User: {userId}, Role: {userRole}, Connection: {connectionId}";
    }
} 