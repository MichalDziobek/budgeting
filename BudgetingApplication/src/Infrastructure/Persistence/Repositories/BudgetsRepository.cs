using Application.Abstractions.Persistence;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

public class BudgetsRepository : GenericRepository<Budget, int>, IBudgetsRepository
{
    public BudgetsRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}