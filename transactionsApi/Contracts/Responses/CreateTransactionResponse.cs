namespace transactionsApi.Contracts.Responses;

public record CreateTransactionResponse(
    Guid TransactionId,
    string CreatedAt
);