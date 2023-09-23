using Application.Abstractions.Persistence;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

public class CategoriesRepository : GenericRepository<Category, int>, ICategoriesRepository
{
    public CategoriesRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}