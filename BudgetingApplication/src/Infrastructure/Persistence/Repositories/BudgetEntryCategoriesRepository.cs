using Application.Abstractions.Persistence;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

public class BudgetEntryCategoriesRepository : GenericRepository<Category, int>, IBudgetEntryCategoriesRepository
{
    public BudgetEntryCategoriesRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}