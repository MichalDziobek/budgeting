namespace WebApi.Tests.Integration.Common.Abstractions;

public interface ITestDatabase
{
    Task<TEntity?> FindAsync<TEntity, TId>(TId id)
        where TEntity : class;

    Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class;

    Task ResetAsync();
}