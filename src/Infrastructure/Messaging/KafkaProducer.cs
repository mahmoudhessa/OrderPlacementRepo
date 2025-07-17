using Confluent.Kafka;
using System.Text.Json;

namespace Talabeyah.OrderManagement.Infrastructure.Messaging;

public class KafkaProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic;

    public KafkaProducer(string bootstrapServers, string topic)
    {
        var config = new ProducerConfig { BootstrapServers = bootstrapServers };
        _producer = new ProducerBuilder<string, string>(config).Build();
        _topic = topic;
    }

    public async Task PublishOrderCreatedAsync(int orderId)
    {
        var message = JsonSerializer.Serialize(new { orderId });
        await _producer.ProduceAsync(_topic, new Message<string, string> { Key = orderId.ToString(), Value = message });
    }
} 