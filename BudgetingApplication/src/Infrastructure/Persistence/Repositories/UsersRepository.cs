using Application.Abstractions.Persistence;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

public class UsersRepository : GenericRepository<User, string>, IUsersRepository
{
    public UsersRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}