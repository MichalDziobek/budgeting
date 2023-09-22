using Application.Abstractions;
using Application.Abstractions.Persistance;
using Application.Exceptions;
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
        if (_currentUserService.UserId is null)
        {
            throw new UnauthorizedException("User does not have valid id in token");
        }

        if (await _usersRepository.GetById(_currentUserService.UserId, cancellationToken) is not null)
        {
            throw new BadRequestException("This user already exists");
        }

        user.Id = _currentUserService.UserId;

        await _usersRepository.Create(user, cancellationToken);
    }
}