using Application.Abstractions.Persistence;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

public class BudgetEntryCategoriesRepository : GenericRepository<BudgetEntryCategory, int>, IBudgetEntryCategoriesRepository
{
    public BudgetEntryCategoriesRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}