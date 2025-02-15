using ErrorOr;
using Infrastructure.Models;
using Infrastructure.Persistence;
using MediatR;
using transactionsApi.Contracts.Results;

namespace transactionsApi.Commands.CreateTransaction;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, ErrorOr<CreateTransactionResult>>
{
    private readonly ILogger<CreateTransactionCommandHandler> _logger;
    private readonly AppDbContext _context;
    public CreateTransactionCommandHandler(
        ILogger<CreateTransactionCommandHandler> logger,
        AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<ErrorOr<CreateTransactionResult>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = new Transaction()
        {
            Id = Guid.NewGuid(),
            SourceAccountId = request.SourceAccountId,
            TargetAccountId = request.TargetAccountId,
            TransferTypeId = request.TranferTypeId,
            Value = request.Value,
            Status = TransactionStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Transaction {transaction.Id} created");

        return new CreateTransactionResult(
            transaction.Id,
            transaction.TargetAccountId,
            transaction.Value,
            transaction.CreatedAt);
    }
}