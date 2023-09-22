using FluentValidation;

namespace Application.Users.Commands;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .MaximumLength(256)
            .NotEmpty();

        
        RuleFor(x => x.FullName)
            .MaximumLength(256)
            .NotEmpty();
    }
}