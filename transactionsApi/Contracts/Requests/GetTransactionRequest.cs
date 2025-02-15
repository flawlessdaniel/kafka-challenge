namespace transactionsApi.Contracts.Requests;

public record GetTransactionRequest(
    Guid transactionExternalId,
    string CreatedAt
);