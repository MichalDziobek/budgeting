using FluentValidation;

namespace Application.BudgetEntries.Commands.UpdateBudgetEntry;

public class UpdateBudgetEntryCommandValidator : AbstractValidator<UpdateBudgetEntryCommand>
{
    public UpdateBudgetEntryCommandValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(256)
            .NotEmpty();

        RuleFor(x => x.BudgetEntryId)
            .NotEmpty();
        
        RuleFor(x => x.CategoryId)
            .NotEmpty();
    }
}