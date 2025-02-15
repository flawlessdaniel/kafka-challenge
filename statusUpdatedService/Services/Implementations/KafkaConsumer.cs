using Confluent.Kafka;
using Infrastructure.Models;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using statusUpdatedService.Services.Abstractions;
using statusUpdatedService.Contracts;
using statusUpdatedService.Services.Common;

namespace statusUpdatedService.Services.Implementations;

public class KafkaConsumer : BackgroundService
{
    private readonly ILogger<KafkaConsumer> _logger;
    private IConsumer<string, string> _transactionsConsumer = default!;
    private readonly IOptions<KafkaSettings> _kafkaSettings;
    private readonly ICacheService _cacheService;
    private readonly AppDbContext _dbContext;
    
    public KafkaConsumer(
        ILogger<KafkaConsumer> logger,
        IOptions<KafkaSettings> kafkaSettings,
        ICacheService cacheService,
        AppDbContext dbContext)
    {
        _logger = logger;
        _kafkaSettings = kafkaSettings;
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
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka Consumer initiated...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var transaction = _transactionsConsumer.Consume(TimeSpan.FromSeconds(1));
                if (transaction != null)
                {
                    _logger.LogInformation($"Transaction received: {transaction.Message.Value}");
                    _transactionsConsumer.Commit(transaction);

                    var transactionData = JsonConvert.DeserializeObject<ValidatedTransaction>(transaction.Message.Value);
                    var transactionObj = await _dbContext.Transactions.FindAsync(transactionData?.TransactionId ?? Guid.Empty);
                    if (transactionObj != null && transactionData != null)
                    {
                        transactionObj.Status = transactionData.Status;
                        await _dbContext.SaveChangesAsync();
                        _logger.LogInformation($"Transaction status updated: {transactionData.TransactionId}");

                        var lockedTransactions = await _cacheService.GetAsync(transaction.Message.Key);
                        if (!string.IsNullOrEmpty(lockedTransactions))
                        {
                            int count = int.Parse(lockedTransactions) - 1;
                            if (count <= 0)
                            {
                                await _cacheService.RemoveAsync(transaction.Message.Key);
                                _logger.LogInformation($"Transaction account removed from cache: {transaction.Message.Key}");
                            }
                            else
                            {
                                await _cacheService.SetAsync(transaction.Message.Key, count.ToString());
                                _logger.LogInformation($"Transaction account updated in cache: {transaction.Message.Key} - {count}");
                            }
                        }

                        if (transactionData.Status is TransactionStatus.Approved)
                        {
                            var summary = await _dbContext.TransactionSummaries.AsNoTracking().FirstOrDefaultAsync(x => x.AccountId == transactionData.AccountId && x.Date.Equals(transactionData.CreatedAt.ToShortDateString()));
                            if (summary is null)
                            {
                                _dbContext.TransactionSummaries.Add(new TransactionSummary
                                {
                                    AccountId = transactionData.AccountId,
                                    Date = transactionData.CreatedAt.ToShortDateString(),
                                    Accumulated = transactionData.Value
                                });
                            }
                            else
                            {
                                summary.Accumulated += transactionData.Value;
                            }
                            await _dbContext.SaveChangesAsync();
                            _logger.LogInformation($"Account summary updated: {transactionData.AccountId} - {transactionData.CreatedAt.ToShortDateString()}");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Consume message failed Status Updated Service: {ex.Message}");
            }
            
            // Delay
            await Task.Delay(100, stoppingToken);
        }
    }

    public override void Dispose()
    {
        _transactionsConsumer.Close();
        _transactionsConsumer.Dispose();
        base.Dispose();
    }
}