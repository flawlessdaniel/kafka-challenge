namespace fraudService.Services.Abstractions;

public interface IKafkaProducer
{
    Task<bool> SendMessageAsync(string topic, string key, string Message);
}