using FluentValidation;

namespace Application.Categories.Queries.GetCategories;

public class GetCategoriesQueryValidator : AbstractValidator<GetCategoriesQuery>
{
    public GetCategoriesQueryValidator()
    {
        RuleFor(x => x.NameSearchQuery)
            .MaximumLength(256).When(x => x.NameSearchQuery is not null);
    }
}