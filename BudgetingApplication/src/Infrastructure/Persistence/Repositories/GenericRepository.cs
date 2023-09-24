using System.Linq.Expressions;
using Application.Abstractions.Persistence;
using Application.DataModels.Common;
using Domain.Entities;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId>
    where TId : notnull
    where TEntity : BaseEntity<TId>
{
    protected readonly ApplicationDbContext _dbContext;
    protected DbSet<TEntity> DbSet => _dbContext.Set<TEntity>();

    protected GenericRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TEntity> Create(TEntity entity, CancellationToken cancellationToken = default)
    {
        DbSet.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<TEntity?> GetById(TId id, CancellationToken cancellationToken = default)
    {
        var entity = await DbSet.FindAsync(new object[] { id }, cancellationToken);
        return entity;
    }

    public Task<bool> Exists(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return DbSet.Where(predicate).AnyAsync(cancellationToken);
    }

    public async Task<List<TEntity>> GetCollection(Func<IQueryable<TEntity>, IQueryable<TEntity>>? filters = null, CancellationToken cancellationToken = default)
    {
        var queryable = DbSet.AsQueryable();

        if (filters is not null)
        {
            queryable = filters(queryable);
        }

        return await queryable.ToListAsync(cancellationToken);
    }

    public async Task<PaginatedResponse<TEntity>> GetPaginatedResponse(int offset, int limit,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? filters = null, CancellationToken cancellationToken = default)
    {
        var queryable = DbSet.AsQueryable();

        if (filters is not null)
        {
            queryable = filters(queryable);
        }

        return await queryable.ToPaginatedListAsync(offset, limit, cancellationToken);
    }

    public async Task<TEntity> Update(TEntity newEntityState, CancellationToken cancellationToken = default)
    {
        DbSet.Update(newEntityState);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return newEntityState;
    }

    public async Task Delete(TEntity entity, CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}