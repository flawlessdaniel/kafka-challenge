using ErrorOr;
using MediatR;
using transactionsApi.Contracts.Results;

namespace transactionsApi.Queries.GetTransactionQuery;

public record GetTransactionQuery(
    Guid Id
) : IRequest<ErrorOr<GetTransactionResult>>;