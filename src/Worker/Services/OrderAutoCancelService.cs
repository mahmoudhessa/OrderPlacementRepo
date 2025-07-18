using Microsoft.EntityFrameworkCore;
using Talabeyah.OrderManagement.Domain.Enums;
using Talabeyah.OrderManagement.Infrastructure;

namespace Talabeyah.OrderManagement.Worker.Services;

public class OrderAutoCancelService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public OrderAutoCancelService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();

            var threshold = DateTime.UtcNow.AddMinutes(-30);
            var staleOrders = await db.Orders
                .Include(o => o.Items)
                .Where(o => o.Status == OrderStatus.Pending && o.CreatedAt < threshold)
                .ToListAsync(stoppingToken);

            foreach (var order in staleOrders)
            {
                order.MarkAsCancelled();
                foreach (var item in order.Items)
                {
                    var product = await db.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId, stoppingToken);
                    if (product != null)
                        product.IncreaseInventory(item.Quantity);
                }
            }

            await db.SaveChangesAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
} 