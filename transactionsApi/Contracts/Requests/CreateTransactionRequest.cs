using MediatR;

namespace transactionsApi.Contracts.Requests;

public record CreateTransactionRequest(
    Guid sourceAccountId,
    Guid targetAccountId,
    int tranferTypeId,
    float value
);