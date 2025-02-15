namespace transactionsApi.Contracts.Results;

public record CreateTransactionResult(
    Guid TransactionId,
    Guid AccountId,
    float Value,
    DateTime CreatedAt
);