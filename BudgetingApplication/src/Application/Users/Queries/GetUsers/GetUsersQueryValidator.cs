using FluentValidation;

namespace Application.Users.Queries.GetUsers;

public class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidator()
    {
        RuleFor(x => x.EmailSearchQuery)
            .MaximumLength(256).When(x => x.EmailSearchQuery is not null);

        RuleFor(x => x.FullNameSearchQuery)
            .MaximumLength(256).When(x => x.FullNameSearchQuery is not null);
    }
}