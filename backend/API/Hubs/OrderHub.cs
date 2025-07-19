using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Talabeyah.OrderManagement.API.Hubs;

public class OrderHub : Talabeyah.OrderManagement.Contracts.OrderHub
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
        if (!string.IsNullOrEmpty(userRole))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userRole);
            _logger.LogInformation("User {UserId} added to group {Group}", userId, userRole);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(System.Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _logger.LogInformation("User {UserId} disconnected from SignalR", userId);
        await base.OnDisconnectedAsync(exception);
    }

    public override async Task JoinOrderUpdates(int orderId)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Order_{orderId}");
        _logger.LogInformation("User {UserId} joined order updates for OrderId {OrderId}", userId, orderId);
    }

    public override async Task LeaveOrderUpdates(int orderId)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Order_{orderId}");
        _logger.LogInformation("User {UserId} left order updates for OrderId {OrderId}", userId, orderId);
    }

    public override async Task JoinInventoryUpdates()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await Groups.AddToGroupAsync(Context.ConnectionId, "InventoryUpdates");
        _logger.LogInformation("User {UserId} joined inventory updates", userId);
    }

    public override async Task LeaveInventoryUpdates()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "InventoryUpdates");
        _logger.LogInformation("User {UserId} left inventory updates", userId);
    }

    public override async Task JoinSystemAlerts()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await Groups.AddToGroupAsync(Context.ConnectionId, "SystemAlerts");
        _logger.LogInformation("User {UserId} joined system alerts", userId);
    }

    public override async Task LeaveSystemAlerts()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SystemAlerts");
        _logger.LogInformation("User {UserId} left system alerts", userId);
    }

    public override async Task<string> GetConnectionInfo()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
        var connectionId = Context.ConnectionId;
        return $"User: {userId}, Role: {userRole}, Connection: {connectionId}";
    }
} 