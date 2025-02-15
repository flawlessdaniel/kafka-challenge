namespace fraudService.Services.Common;

public class KafkaSettings
{
    public string BootstrapServers { get; set; }
    public string ConsumeGroupId { get; set; }
    public string ConsumeTopic { get; set; }
    public string RetriesGroupId { get; set; }
    public string RetriesTopic { get; set; }
    public string PublishTopic { get; set; }
}