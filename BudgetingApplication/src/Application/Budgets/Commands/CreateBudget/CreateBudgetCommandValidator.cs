using FluentValidation;

namespace Application.Budgets.Commands.CreateBudget;

public class CreateBudgetCommandValidator : AbstractValidator<CreateBudgetCommand>
{
    public CreateBudgetCommandValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(256)
            .NotEmpty();
    }
}