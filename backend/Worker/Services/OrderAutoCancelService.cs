using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Talabeyah.OrderManagement.Domain.Enums;
using Talabeyah.OrderManagement.Infrastructure.Persistence;
using Talabeyah.OrderManagement.Domain.Services;

namespace Talabeyah.OrderManagement.Worker.Services;

public class OrderAutoCancelService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OrderAutoCancelService> _logger;

    public OrderAutoCancelService(IServiceProvider serviceProvider, ILogger<OrderAutoCancelService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Order Auto-Cancel Service...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();
                var orderDomainService = scope.ServiceProvider.GetRequiredService<OrderDomainService>();
                var productDomainService = scope.ServiceProvider.GetRequiredService<ProductDomainService>();

                var threshold = DateTime.UtcNow.AddMinutes(-30);
                var staleOrders = await db.Orders
                    .Include(o => o.Items)
                    .Where(o => o.Status == OrderStatus.Pending && o.CreatedAt < threshold)
                    .ToListAsync(stoppingToken);

                if (staleOrders.Any())
                {
                    _logger.LogInformation("Found {Count} stale orders to process", staleOrders.Count);
                }

                foreach (var order in staleOrders)
                {
                    if (order.Status != OrderStatus.Pending)
                    {
                        _logger.LogDebug("Order {OrderId} already processed. Skipping.", order.Id);
                        continue;
                    }

                    try
                    {
                        orderDomainService.MarkAsCancelled(order);
                        
                        foreach (var item in order.Items)
                        {
                            var product = await db.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId, stoppingToken);
                            if (product != null)
                            {
                                productDomainService.IncreaseInventory(product, item.Quantity);
                                _logger.LogInformation("Increased inventory for product {ProductId} by {Quantity}", product.Id, item.Quantity);
                            }
                        }

                        // Log cancellation
                        db.AuditLogs.Add(new Domain.Entities.AuditLog($"Order {order.Id} auto-cancelled by worker at {DateTime.UtcNow:O}"));
                        
                        _logger.LogInformation("Order {OrderId} auto-cancelled successfully", order.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing stale order {OrderId}", order.Id);
                    }
                }

                if (staleOrders.Any())
                {
                    await db.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("Saved changes for {Count} cancelled orders", staleOrders.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Order Auto-Cancel Service");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }

        _logger.LogInformation("Order Auto-Cancel Service stopped");
    }
} 