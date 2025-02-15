using Infrastructure.Models;

namespace statusUpdatedService.Contracts;

public record ValidatedTransaction(
    Guid TransactionId,
    Guid AccountId,
    float Value,
    DateTime CreatedAt,
    TransactionStatus Status
);