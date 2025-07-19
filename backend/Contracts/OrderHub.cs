using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Talabeyah.OrderManagement.Contracts;

public class OrderHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(System.Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    public virtual async Task JoinOrderUpdates(int orderId) { await Task.CompletedTask; }
    public virtual async Task LeaveOrderUpdates(int orderId) { await Task.CompletedTask; }
    public virtual async Task JoinInventoryUpdates() { await Task.CompletedTask; }
    public virtual async Task LeaveInventoryUpdates() { await Task.CompletedTask; }
    public virtual async Task JoinSystemAlerts() { await Task.CompletedTask; }
    public virtual async Task LeaveSystemAlerts() { await Task.CompletedTask; }
    public virtual async Task<string> GetConnectionInfo() { return string.Empty; }
} 