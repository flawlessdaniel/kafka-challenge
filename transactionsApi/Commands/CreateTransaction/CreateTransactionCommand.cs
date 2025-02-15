using ErrorOr;
using MediatR;
using transactionsApi.Contracts.Results;

namespace transactionsApi.Commands;

public record CreateTransactionCommand(
    Guid SourceAccountId,
    Guid TargetAccountId,
    int TranferTypeId,
    float Value
) : IRequest<ErrorOr<CreateTransactionResult>>;