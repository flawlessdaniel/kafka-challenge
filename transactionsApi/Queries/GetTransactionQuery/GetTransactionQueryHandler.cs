using ErrorOr;
using Infrastructure.Persistence;
using MediatR;
using transactionsApi.Contracts.Results;

namespace transactionsApi.Queries.GetTransactionQuery;

public class GetTransactionQueryHandler : IRequestHandler<GetTransactionQuery, ErrorOr<GetTransactionResult>>
{
    private readonly AppDbContext _dbContext;

    public GetTransactionQueryHandler(
        AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<GetTransactionResult>> Handle(GetTransactionQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _dbContext.Transactions.FindAsync(request.Id);
        if (transaction == null)
        {
            return ErrorOr.Error.NotFound("Transaction not found");
        }
        
        return new GetTransactionResult(
            transaction.Id,
            transaction.SourceAccountId,
            transaction.TargetAccountId,
            transaction.TransferTypeId,
            transaction.Value,
            transaction.Status,
            transaction.CreatedAt);
    }
}