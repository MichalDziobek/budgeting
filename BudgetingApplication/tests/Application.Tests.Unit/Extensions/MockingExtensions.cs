using System.Linq.Expressions;
using Application.Abstractions.Persistence;
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
        return repository.GetCollection(Arg.Any<Expression<Func<TEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(info =>
            {
                var predicate = info.Arg<Expression<Func<TEntity, bool>>>()?.Compile();
                return predicate is null ? mockData.ToList() : mockData.Where(x => predicate(x)).ToList();
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