using Confluent.Kafka;
using fraudService.Contracts;
using fraudService.Services.Abstractions;
using fraudService.Services.Common;
using Infrastructure.Models;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace fraudService.Services.Implementation;

public class KafkaConsumer : BackgroundService
{
    private readonly ILogger<KafkaConsumer> _logger;
    private IConsumer<string, string> _transactionsConsumer = default!;
    private IConsumer<string, string> _transactionsRetriesConsumer = default!;
    private readonly IOptions<KafkaSettings> _kafkaSettings;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly ICacheService _cacheService;
    private readonly AppDbContext _dbContext;

    public KafkaConsumer(
        ILogger<KafkaConsumer> logger,
        IOptions<KafkaSettings> kafkaSettings,
        IKafkaProducer producer,
        ICacheService cacheService,
        AppDbContext dbContext)
    {
        _logger = logger;
        _kafkaSettings = kafkaSettings;
        _kafkaProducer = producer;
        _cacheService = cacheService;
        _dbContext = dbContext;

        SetUpConsumer();
    }

    private void SetUpConsumer()
    {
        var transactionsConfig = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.Value.BootstrapServers,
            GroupId = _kafkaSettings.Value.ConsumeGroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        _transactionsConsumer = new ConsumerBuilder<string, string>(transactionsConfig).Build();
        _transactionsConsumer.Subscribe(_kafkaSettings.Value.ConsumeTopic);

        var transactionRetriesConfig = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.Value.BootstrapServers,
            GroupId = _kafkaSettings.Value.RetriesGroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        _transactionsRetriesConsumer = new ConsumerBuilder<string, string>(transactionRetriesConfig).Build();
        _transactionsRetriesConsumer.Subscribe(_kafkaSettings.Value.RetriesTopic);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka Consumer initiated...");

        while (!stoppingToken.IsCancellationRequested)
        {
            // Process retries
            do
            {
                try
                {
                    var result = _transactionsRetriesConsumer.Consume(TimeSpan.FromSeconds(1));
                    if (result != null)
                    {
                        _logger.LogInformation($"Retry transaction received: {result.Message.Value}");
                        _transactionsRetriesConsumer.Commit(result);

                        if (await IsLocked(result.Message.Key))
                        {
                            await SendToRetryTopic(result.Message.Key, result.Message.Value);
                            break;
                        }
                        else
                        {
                            await SendToTransactionsTopic(result.Message.Key, result.Message.Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Consume from retries failed: {ex.Message}");
                }
            }
            while (HasMessages(_transactionsRetriesConsumer));

            // Process transactions
            try 
            {
                var transaction = _transactionsConsumer.Consume(TimeSpan.FromSeconds(1));
                if (transaction != null)
                {
                    _logger.LogInformation($"Transaction received: {transaction.Message.Value}");
                    _transactionsConsumer.Commit(transaction);

                    if (await IsLocked(transaction.Message.Key))
                    {
                        await SendToRetryTopic(transaction.Message.Key, transaction.Message.Value);
                    }
                    else
                    {
                        await LockTransaction(transaction.Message.Key);
                        await ProcessTransaction(transaction.Message.Key, transaction.Message.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Consume from transactions failed: {ex.Message}");
            }

            // Delay
            await Task.Delay(100, stoppingToken);
        }
    }

    private bool HasMessages(IConsumer<string, string> consumer)
    {
        if (consumer.Assignment.Count == 0) return false;
        
        foreach (var partition in consumer.Assignment)
        {
            var watermarkOffsets = _transactionsConsumer.QueryWatermarkOffsets(partition, TimeSpan.FromSeconds(1));
            var currentPosition = _transactionsConsumer.Position(partition);

            if (currentPosition < watermarkOffsets.High) return true;
        }

        return false;
    }

    private async Task ProcessTransaction(string key, string transactionMessage)
    {
        var transaction = JsonConvert.DeserializeObject<Contracts.Transaction>(transactionMessage);
        if (transaction == null) return;

        ValidatedTransaction result = new(
            transaction.TransactionId,
            transaction.AccountId,
            transaction.Value,
            transaction.CreatedAt,
            TransactionStatus.Approved
        );

        if (transaction.Value > 2000)
        {
            result = result with { Status = TransactionStatus.Rejected };
        }

        string transactionDate = transaction.CreatedAt.ToShortDateString();
        var accountSummary = await _dbContext.TransactionSummaries.AsNoTracking().FirstOrDefaultAsync(x => x.AccountId == transaction.AccountId && x.Date.Equals(transactionDate));
        _logger.LogInformation($"Transaction fraud validation: accumulated: {accountSummary?.Accumulated}, current: {transaction.Value}");
        if (accountSummary != null && (accountSummary.Accumulated + transaction.Value) > 20000)
        {
            result = result with { Status = TransactionStatus.Rejected };
        }

        _logger.LogInformation($"Transaction fraud validation processed: {result}");
        await SendToStatusUpdateTopic(key, JsonConvert.SerializeObject(result));
    }

    private async Task<bool> IsLocked(string key)
    {
        var val = await _cacheService.GetAsync(key);
        return !string.IsNullOrEmpty(val) && int.Parse(val) > 0;
    }

    private async Task LockTransaction(string key)
    {
        int incidences = 0;
        var val = await _cacheService.GetAsync(key);
        if (!string.IsNullOrEmpty(val)) incidences = int.Parse(val) + 1;
        await _cacheService.SetAsync(key, incidences.ToString());
        _logger.LogInformation($"Transaction locked: {key}");
    }

    private async Task SendToRetryTopic(string key, string transactionMessage)
    {
        await _kafkaProducer.SendMessageAsync(_kafkaSettings.Value.RetriesTopic, key, transactionMessage);
        _logger.LogInformation($"Transaction sent to retry: {key}");
    }

    private async Task SendToStatusUpdateTopic(string key, string transactionMessage)
    {
        await _kafkaProducer.SendMessageAsync(_kafkaSettings.Value.PublishTopic, key, transactionMessage);
        _logger.LogInformation($"Transaction sent to status update: {key}");
    }

    private async Task SendToTransactionsTopic(string key, string transactionMessage)
    {
        await _kafkaProducer.SendMessageAsync(_kafkaSettings.Value.ConsumeTopic, key, transactionMessage);
        _logger.LogInformation($"Transaction sent back to transactions: {key}");
    }

    public override void Dispose()
    {
        _transactionsConsumer.Close();
        _transactionsConsumer.Dispose();
        _transactionsRetriesConsumer.Close();
        _transactionsRetriesConsumer.Dispose();
        base.Dispose();
    }
}