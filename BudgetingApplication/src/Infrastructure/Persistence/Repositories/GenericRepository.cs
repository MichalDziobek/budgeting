using System.Linq.Expressions;
using Application.Abstractions.Persistance;
using Application.DataModels.Common;
using Domain.Entities;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId>
    where TId : notnull
    where TEntity : BaseEntity<TId>
{
    private readonly ApplicationDbContext _dbContext;
    protected DbSet<TEntity> DbSet => _dbContext.Set<TEntity>();

    public GenericRepository(ApplicationDbContext dbContext)
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
        return await DbSet.FindAsync(new[] { id }, cancellationToken);
    }

    public async Task<PaginatedResponse<TEntity>> GetPaginatedResponse(int offset, int limit,
        Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var queryable = DbSet.AsQueryable();

        if (predicate is not null)
        {
            queryable = queryable.Where(predicate);
        }

        return await queryable.ToPaginatedListAsync(offset, limit, cancellationToken);
    }

    public async Task<TEntity> Update(TEntity newEntityState, CancellationToken cancellationToken = default)
    {
        DbSet.Update(newEntityState);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return newEntityState;
    }

    public async Task Delete(TId id, CancellationToken cancellationToken = default)
    {
        var entity = await GetById(id, cancellationToken);
        if (entity is null)
        {
            return;
        }

        DbSet.Remove(entity);
    }
}