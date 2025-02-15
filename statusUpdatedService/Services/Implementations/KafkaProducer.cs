using Confluent.Kafka;
using Microsoft.Extensions.Options;
using statusUpdatedService.Services.Abstractions;
using statusUpdatedService.Services.Common;

namespace statusUpdatedService.Services.Implementations;

public class KafkaProducer : IKafkaProducer
{
    private readonly IOptions<KafkaSettings> _settings;
    private readonly IProducer<string, string> _producer;

    public KafkaProducer(
        IOptions<KafkaSettings> settings)
    {
        _settings = settings;

        var config = new ProducerConfig
        {
            BootstrapServers = _settings.Value.BootstrapServers,
            Acks = Acks.All,
            LingerMs = 5,
            CompressionType = CompressionType.Snappy,
            AllowAutoCreateTopics = true
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task<bool> SendMessageAsync(string topic, string key, string message)
    {
        var result = await _producer.ProduceAsync(topic, new Message<string, string>
        {
            Key = key,
            Value = message
        });

        return result.Status == PersistenceStatus.Persisted;
    }
}