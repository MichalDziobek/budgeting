using System.Linq.Expressions;
using Application.DataModels.Common;
using Domain.Entities;

namespace Application.Abstractions.Persistence;

public interface IGenericRepository<TEntity, in TId>
    where TId : notnull
    where TEntity : BaseEntity<TId>
{
    public Task<TEntity> Create(TEntity entity, CancellationToken cancellationToken = default);
    public Task<TEntity?> GetById(TId id, CancellationToken cancellationToken = default);
    public Task<bool> Exists(Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    public Task<List<TEntity>> GetCollection(Func<IQueryable<TEntity>, IQueryable<TEntity>>? filters = null,
        CancellationToken cancellationToken = default);
    public Task<PaginatedResponse<TEntity>> GetPaginatedResponse(int offset, int limit, 
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? filters = null, CancellationToken cancellationToken = default);
    public Task<TEntity> Update(TEntity newEntityState, CancellationToken cancellationToken = default);
    public Task Delete(TEntity entity, CancellationToken cancellationToken = default);

}