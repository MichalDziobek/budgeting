using Application.DataModels.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Extensions;

public static class PaginationExtensions
{
    public static async Task<PaginatedResponse<TDestination>> ToPaginatedListAsync<TDestination>(
        this IQueryable<TDestination> queryable, int offset, int limit, CancellationToken cancellationToken = default) where TDestination : class
    {
        var count = await queryable.CountAsync(cancellationToken);
        var items = await queryable.Skip(offset).Take(limit).ToListAsync(cancellationToken);
        return new PaginatedResponse<TDestination>(items, count);
    }
}