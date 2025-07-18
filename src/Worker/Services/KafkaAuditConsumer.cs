using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Talabeyah.OrderManagement.Infrastructure;

namespace Talabeyah.OrderManagement.Worker.Services;

public class KafkaAuditConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly string _bootstrapServers;
    private readonly string _topic;

    public KafkaAuditConsumer(IServiceProvider serviceProvider, IConfiguration config)
    {
        _serviceProvider = serviceProvider;
        _bootstrapServers = config["Kafka:BootstrapServers"] ?? "localhost:9092";
        _topic = "order-created";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = "audit-worker",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(_topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var cr = consumer.Consume(stoppingToken);
                var data = JsonSerializer.Deserialize<OrderCreatedMessage>(cr.Message.Value);
                if (data != null)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();
                    db.AuditLogs.Add(new Domain.Entities.AuditLog($"Order with ID {data.orderId} created"));
                    await db.SaveChangesAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException) { }
        }
    }

    private record OrderCreatedMessage(int orderId);
} 