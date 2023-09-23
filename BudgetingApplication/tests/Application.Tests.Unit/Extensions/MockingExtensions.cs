using System.Linq.Expressions;
using Application.Abstractions.Persistence;
using Application.DataModels.Common;
using Domain.Entities;
using NSubstitute.Core;

namespace Application.Tests.Unit.Extensions;

public static class MockingExtensions
{
    public static ConfiguredCall MockGetCollection<TEntity, TId>(this IGenericRepository<TEntity, TId> repository,
        IEnumerable<TEntity> mockData)
        where TId : notnull
        where TEntity : BaseEntity<TId>
    {
        return repository.GetCollection(Arg.Any<Func<IQueryable<TEntity>, IQueryable<TEntity>>>(), Arg.Any<CancellationToken>())
            .Returns(info =>
            {
                var filters = info.Arg<Func<IQueryable<TEntity>, IQueryable<TEntity>>>();
                return filters is null ? mockData.ToList() : filters(mockData.AsQueryable()).ToList();
            });
    }
    
    public static ConfiguredCall MockGetPaginatedCollection<TEntity, TId>(this IGenericRepository<TEntity, TId> repository,
        IEnumerable<TEntity> mockData)
        where TId : notnull
        where TEntity : BaseEntity<TId>
    {
        return repository.GetPaginatedResponse(
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Func<IQueryable<TEntity>, IQueryable<TEntity>>>(),
                Arg.Any<CancellationToken>())
            .Returns(info =>
            {
                var filters = info.Arg<Func<IQueryable<TEntity>, IQueryable<TEntity>>>();
                var offset = info.ArgAt<int>(0);
                var limit = info.ArgAt<int>(1);
                return filters is null
                    ? PaginatedResponse<TEntity>.From(mockData.Skip(offset).Take(limit))
                    : PaginatedResponse<TEntity>.From(filters(mockData.AsQueryable()).Skip(offset).Take(limit).ToList());
            });
    }
    
    public static ConfiguredCall MockExists<TEntity, TId>(this IGenericRepository<TEntity, TId> repository,
        IEnumerable<TEntity> mockData)
        where TId : notnull
        where TEntity : BaseEntity<TId>
    {
        return repository.Exists(Arg.Any<Expression<Func<TEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(info =>
            {
                var predicate = info.Arg<Expression<Func<TEntity, bool>>>()?.Compile();
                return predicate is not null && mockData.Any(x => predicate(x));
            });
    }
}