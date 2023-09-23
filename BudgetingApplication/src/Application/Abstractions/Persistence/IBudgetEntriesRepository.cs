using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IBudgetEntriesRepository : IGenericRepository<BudgetEntry, int> { }