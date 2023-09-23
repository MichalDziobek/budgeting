using FluentValidation;

namespace Application.BudgetEntries.Queries.GetBudgetEntries;

public class GetBudgetEntriesQueryValidator : AbstractValidator<GetBudgetEntriesQuery>
{
    public GetBudgetEntriesQueryValidator()
    {
        RuleFor(x => x.BudgetId)
            .NotEmpty();

        RuleFor(x => x.Limit)
            .GreaterThan(0);

        RuleFor(x => x.Offset)
            .GreaterThanOrEqualTo(0);
    }
}