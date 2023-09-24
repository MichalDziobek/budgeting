using Application.Abstractions.Persistence;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

public class BudgetsRepository : GenericRepository<Budget, int>, IBudgetsRepository
{
    public BudgetsRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task AddSharedBudget(SharedBudget sharedBudget, CancellationToken cancellationToken)
    {
        DbContext.SharedBudgets.Add(sharedBudget);
        await DbContext.SaveChangesAsync(cancellationToken);
    }
}