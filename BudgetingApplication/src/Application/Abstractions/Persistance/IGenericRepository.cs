using System.Linq.Expressions;
using Application.DataModels.Common;
using Domain.Entities;

namespace Application.Abstractions.Persistance;

public interface IGenericRepository<TEntity, in TId>
    where TId : notnull
    where TEntity : BaseEntity<TId>
{
    public Task<TEntity> Create(TEntity entity, CancellationToken cancellationToken = default);
    public Task<TEntity?> GetById(TId id, CancellationToken cancellationToken = default);
    public Task<PaginatedResponse<TEntity>> GetPaginatedResponse(int offset, int limit, 
        Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
    public Task<TEntity> Update(TEntity newEntityState, CancellationToken cancellationToken = default);
    public Task Delete(TId id, CancellationToken cancellationToken = default);

}