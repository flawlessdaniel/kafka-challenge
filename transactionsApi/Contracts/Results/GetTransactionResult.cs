using Infrastructure.Models;

namespace transactionsApi.Contracts.Results;

public record GetTransactionResult(
    Guid TransactionExternalId,
    Guid SourceAccountId,
    Guid TargetAccountId,
    int TransferTypeId,
    float Value,
    TransactionStatus Status,
    DateTime CreatedAt
);