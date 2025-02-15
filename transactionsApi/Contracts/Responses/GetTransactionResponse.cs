namespace transactionsApi.Contracts.Responses;

public record GetTransactionResponse(
    Guid TransactionExternalId,
    Guid SourceAccountId,
    Guid TargetAccountId,
    int TransferTypeId,
    float Value,
    string Status,
    string CreatedAt
);