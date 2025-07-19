using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Talabeyah.OrderManagement.Infrastructure.Services;

public class KafkaTopicInitializer
{
    private readonly ILogger<KafkaTopicInitializer> _logger;
    private readonly string _bootstrapServers;

    public KafkaTopicInitializer(ILogger<KafkaTopicInitializer> logger, IConfiguration configuration)
    {
        _logger = logger;
        _bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "kafka:29092";
    }

    public async Task InitializeTopicsAsync()
    {
        try
        {
            _logger.LogInformation("Initializing Kafka topics...");

            var config = new AdminClientConfig
            {
                BootstrapServers = _bootstrapServers
            };

            using var adminClient = new AdminClientBuilder(config).Build();

            var topics = new[]
            {
                new TopicSpecification
                {
                    Name = "order-created",
                    ReplicationFactor = 1,
                    NumPartitions = 1
                },
                new TopicSpecification
                {
                    Name = "order-updated",
                    ReplicationFactor = 1,
                    NumPartitions = 1
                },
                new TopicSpecification
                {
                    Name = "order-cancelled",
                    ReplicationFactor = 1,
                    NumPartitions = 1
                }
            };

            try
            {
                await adminClient.CreateTopicsAsync(topics);
                _logger.LogInformation("Kafka topics created successfully");
            }
            catch (CreateTopicsException ex) when (ex.Message.Contains("already exists"))
            {
                _logger.LogInformation("Kafka topics already exist, skipping creation");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to create Kafka topics: {Message}", ex.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing Kafka topics");
        }
    }
} 