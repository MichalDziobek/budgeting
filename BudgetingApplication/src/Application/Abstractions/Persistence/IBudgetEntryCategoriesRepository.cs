using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IBudgetEntryCategoriesRepository : IGenericRepository<Category, int> { }