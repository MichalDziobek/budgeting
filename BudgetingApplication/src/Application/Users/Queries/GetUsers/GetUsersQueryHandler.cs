using System.Linq.Expressions;
using Application.Abstractions.Persistence;
using Application.Users.Queries.DataModels;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, GetUsersResponse>
{
    private readonly IUsersRepository _usersRepository;

    public GetUsersQueryHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<GetUsersResponse> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<User, bool>>? predicate = request switch
        {
            { EmailSearchQuery: not null, FullNameSearchQuery: not null } =>
                user => user.FullName.Contains(request.FullNameSearchQuery) && user.Email.Contains(request.EmailSearchQuery),
            { EmailSearchQuery: not null } => user => user.Email.Contains(request.EmailSearchQuery),
            { FullNameSearchQuery: not null } =>  user => user.FullName.Contains(request.FullNameSearchQuery),
            _ => null
            
        };

        var userDtos = (await _usersRepository.GetCollection(predicate, cancellationToken)).Adapt<IEnumerable<UserDto>>();
        var response = new GetUsersResponse
        {
            Users = userDtos
        };
        return response;
    }
}