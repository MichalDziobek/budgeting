using Domain.Entities;

namespace WebApi.Tests.Integration.Common.Abstractions;

public interface ITestDatabase
{
    Task<TEntity?> FindAsync<TEntity, TId>(TId id)
        where TId : notnull
        where TEntity : BaseEntity<TId>;

    Task<TEntity> AddAsync<TEntity, TId>(TEntity entity)
        where TId : notnull
        where TEntity : BaseEntity<TId>;
    
    Task AddRangeAsync<TEntity, TId>(IEnumerable<TEntity> entity)
        where TId : notnull
        where TEntity : BaseEntity<TId>;

    Task ResetAsync();
}