using Application.Abstractions.Persistence;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

public class BudgetEntriesRepository : GenericRepository<BudgetEntry, int>, IBudgetEntriesRepository
{
    public BudgetEntriesRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}