using FluentValidation;

namespace Application.Budgets.Commands.ShareBudgetCommand;

public class ShareBudgetCommandValidator : AbstractValidator<ShareBudgetCommand>
{
    public ShareBudgetCommandValidator()
    {
        RuleFor(x => x.SharedUserId)
            .NotEmpty();

        RuleFor(x => x.BudgetId)
            .NotEmpty();
    }
}