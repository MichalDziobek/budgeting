using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IBudgetsRepository : IGenericRepository<Budget, int> { }