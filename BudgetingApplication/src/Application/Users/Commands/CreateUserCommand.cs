using MediatR;

namespace Application.Users.Commands;

public class CreateUserCommand : IRequest
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}