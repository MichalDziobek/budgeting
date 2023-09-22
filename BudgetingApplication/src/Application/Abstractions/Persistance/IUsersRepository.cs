using Domain.Entities;

namespace Application.Abstractions.Persistance;

public interface IUsersRepository : IGenericRepository<User, string>
{
    
}