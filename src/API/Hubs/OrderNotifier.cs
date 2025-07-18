using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Talabeyah.OrderManagement.Application.Orders.Commands;

namespace Talabeyah.OrderManagement.API.Hubs;

public class OrderNotifier : IOrderNotifier
{
    private readonly IHubContext<OrderHub> _hubContext;
    public OrderNotifier(IHubContext<OrderHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyOrderCreatedAsync(int orderId, DateTime createdAt, string status, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.All.SendAsync(
            "OrderCreated",
            new { orderId, createdAt, status },
            cancellationToken
        );
    }
} 