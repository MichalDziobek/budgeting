using Application.Abstractions;
using Application.Abstractions.Persistance;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Users.Commands;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUsersRepository _usersRepository;

    public CreateUserCommandHandler(ICurrentUserService currentUserService, IUsersRepository usersRepository)
    {
        _currentUserService = currentUserService;
        _usersRepository = usersRepository;
    }

    public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = request.Adapt<User>();
        user.Id = _currentUserService.UserId!; //TODO Handle null

        await _usersRepository.Create(user, cancellationToken);
    }
}