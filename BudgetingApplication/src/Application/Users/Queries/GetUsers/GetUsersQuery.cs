using MediatR;

namespace Application.Users.Queries.GetUsers;

public class GetUsersQuery : IRequest<GetUsersResponse>
{
    public string? FullNameSearchQuery { get; set; }
    public string? EmailSearchQuery { get; set; }
}