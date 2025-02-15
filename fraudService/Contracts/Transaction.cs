using Infrastructure.Models;

namespace fraudService.Contracts;

public record Transaction(
    Guid TransactionId,
    Guid AccountId,
    float Value,
    DateTime CreatedAt
);

public record ValidatedTransaction(
    Guid TransactionId,
    Guid AccountId,
    float Value,
    DateTime CreatedAt,
    TransactionStatus Status
);