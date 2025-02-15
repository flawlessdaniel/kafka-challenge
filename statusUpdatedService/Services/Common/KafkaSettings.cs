namespace statusUpdatedService.Services.Common;

public class KafkaSettings
{
    public string BootstrapServers { get; set; }
    public string ConsumeGroupId { get; set; }
    public string ConsumeTopic { get; set; }
}