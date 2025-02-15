using FluentValidation;

namespace transactionsApi.Commands.CreateTransaction;

public class CreateTransactionValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionValidator()
    {
        RuleFor(x => x.SourceAccountId).NotEmpty();
        RuleFor(x => x.TargetAccountId).NotEmpty();
        RuleFor(x => x.TranferTypeId).NotEmpty();
        RuleFor(x => x.Value).NotEmpty();
    }
}