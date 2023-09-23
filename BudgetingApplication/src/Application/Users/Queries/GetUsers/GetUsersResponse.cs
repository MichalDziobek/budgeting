using Application.Users.Queries.DataModels;

namespace Application.Users.Queries.GetUsers;

public class GetUsersResponse
{
    public IEnumerable<UserDto> Users { get; set; } = Enumerable.Empty<UserDto>();
}