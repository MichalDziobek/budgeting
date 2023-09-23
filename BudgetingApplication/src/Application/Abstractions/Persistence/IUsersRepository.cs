using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IUsersRepository : IGenericRepository<User, string> { }