namespace transactionsApi.Services.Kafka;

public interface IKafkaProducer
{
    Task<bool> SendMessageAsync(string key, string Message);
}