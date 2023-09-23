using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface ICategoriesRepository : IGenericRepository<Category, int> { }