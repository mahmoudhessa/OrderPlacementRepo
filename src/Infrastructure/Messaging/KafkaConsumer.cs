using Confluent.Kafka;
using System.Text.Json;

namespace Talabeyah.OrderManagement.Infrastructure.Messaging;

public class KafkaConsumer
{
    private readonly IConsumer<string, string> _consumer;
    private readonly string _topic;

    public KafkaConsumer(string bootstrapServers, string topic, string groupId)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        _consumer = new ConsumerBuilder<string, string>(config).Build();
        _topic = topic;
    }

    public void StartConsuming(Func<int, Task> onOrderCreated, CancellationToken cancellationToken)
    {
        _consumer.Subscribe(_topic);
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var cr = _consumer.Consume(cancellationToken);
                var data = JsonSerializer.Deserialize<OrderCreatedMessage>(cr.Message.Value);
                if (data != null)
                {
                    onOrderCreated(data.orderId).Wait();
                }
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            _consumer.Close();
        }
    }

    private record OrderCreatedMessage(int orderId);
} 