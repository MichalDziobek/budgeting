using System.Linq.Expressions;
using Application.Abstractions.Persistence;
using Application.Users.DataModels;
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
        var filters = GetFilters(request);

        var userDtos = (await _usersRepository.GetCollection(filters, cancellationToken)).Adapt<IEnumerable<UserDto>>();
        var response = new GetUsersResponse
        {
            Users = userDtos
        };
        return response;
    }

    private static Func<IQueryable<User>, IQueryable<User>> GetFilters(GetUsersQuery request)
    {
        Func<IQueryable<User>, IQueryable<User>> filters = users => users;
        if (request.EmailSearchQuery is not null)
        {
            filters = users => users.Where(user => user.Email.Contains(request.EmailSearchQuery));
        }

        if (request.FullNameSearchQuery is not null)
        {
            var filtersCopy = filters;
            filters = users => filtersCopy(users).Where(user => user.FullName.Contains(request.FullNameSearchQuery));
        }

        return filters;
    }
}