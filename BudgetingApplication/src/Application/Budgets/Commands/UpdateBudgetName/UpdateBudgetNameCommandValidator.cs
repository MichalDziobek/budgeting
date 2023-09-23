using FluentValidation;

namespace Application.Budgets.Commands.UpdateBudgetNameCommand;

public class UpdateBudgetNameCommandValidator : AbstractValidator<UpdateBudgetNameCommand>
{
    public UpdateBudgetNameCommandValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(256)
            .NotEmpty();

        RuleFor(x => x.BudgetId)
            .NotEmpty();
    }
}