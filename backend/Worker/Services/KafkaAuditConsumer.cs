using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Talabeyah.OrderManagement.Infrastructure.Persistence;

namespace Talabeyah.OrderManagement.Worker.Services;

public class KafkaAuditConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly string _bootstrapServers;
    private readonly string _topic;
    private readonly ILogger<KafkaAuditConsumer> _logger;

    public KafkaAuditConsumer(IServiceProvider serviceProvider, IConfiguration config, ILogger<KafkaAuditConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _bootstrapServers = config?["Kafka:BootstrapServers"] ?? "kafka:9092";
        _topic = "order-created";
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Kafka Audit Consumer...");
        _logger.LogInformation("Connecting to Kafka at: {BootstrapServers}", _bootstrapServers);

        // Wait for Kafka to be ready
        await WaitForKafkaReady(stoppingToken);

        var config = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = "audit-worker",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true,
            AutoCommitIntervalMs = 5000,
            SessionTimeoutMs = 30000,
            HeartbeatIntervalMs = 10000
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(_topic);

        _logger.LogInformation("Kafka consumer started and subscribed to topic: {Topic}", _topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var cr = consumer.Consume(TimeSpan.FromSeconds(10));
                if (cr != null)
                {
                    _logger.LogInformation("Received message from Kafka: {Message}", cr.Message.Value);
                    var data = JsonSerializer.Deserialize<OrderCreatedMessage>(cr.Message.Value);
                    if (data != null)
                    {
                        using var scope = _serviceProvider?.CreateScope();
                        if (scope != null)
                        {
                            var db = scope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();
                            // Idempotency check: skip if audit log for this order already exists
                            bool alreadyLogged = await db.AuditLogs.AnyAsync(a => a.Change.Contains($"Order with ID {data.orderId} created"), stoppingToken);
                            if (alreadyLogged)
                            {
                                _logger.LogInformation("Audit log already exists for order {OrderId}, skipping", data.orderId);
                                continue;
                            }
                            db.AuditLogs.Add(new Domain.Entities.AuditLog($"Order with ID {data.orderId} created"));
                            await db.SaveChangesAsync(stoppingToken);
                            _logger.LogInformation("Audit log created for order {OrderId}", data.orderId);
                        }
                    }
                }
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Error consuming message from Kafka");
                await Task.Delay(5000, stoppingToken); // Wait before retrying
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Kafka consumer operation cancelled");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Kafka consumer");
                await Task.Delay(5000, stoppingToken); // Wait before retrying
            }
        }

        _logger.LogInformation("Kafka consumer stopped");
    }

    private async Task WaitForKafkaReady(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Waiting for Kafka to be ready...");
        var maxRetries = 30;
        var retryCount = 0;

        while (retryCount < maxRetries && !stoppingToken.IsCancellationRequested)
        {
            try
            {
                var config = new ProducerConfig
                {
                    BootstrapServers = _bootstrapServers,
                    ClientId = "worker-health-check"
                };

                using var producer = new ProducerBuilder<string, string>(config).Build();
                var result = await producer.ProduceAsync("health-check", new Message<string, string> { Value = "ping" });
                _logger.LogInformation("Kafka is ready!");
                return;
            }
            catch (Exception ex)
            {
                retryCount++;
                _logger.LogWarning("Kafka not ready yet (attempt {Attempt}/{MaxAttempts}): {Error}", retryCount, maxRetries, ex.Message);
                await Task.Delay(2000, stoppingToken);
            }
        }

        if (retryCount >= maxRetries)
        {
            _logger.LogError("Kafka failed to become ready after {MaxRetries} attempts", maxRetries);
            throw new InvalidOperationException("Kafka is not available");
        }
    }

    private record OrderCreatedMessage(int orderId);
} 