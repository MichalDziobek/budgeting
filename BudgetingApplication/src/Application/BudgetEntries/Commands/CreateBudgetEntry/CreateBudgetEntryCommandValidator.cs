using FluentValidation;

namespace Application.BudgetEntries.Commands.CreateBudgetEntry;

public class CreateBudgetEntryCommandValidator : AbstractValidator<CreateBudgetEntryCommand>
{
    public CreateBudgetEntryCommandValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(256)
            .NotEmpty();

        RuleFor(x => x.BudgetId)
            .NotEmpty();
        
        RuleFor(x => x.CategoryId)
            .NotEmpty();
    }
}