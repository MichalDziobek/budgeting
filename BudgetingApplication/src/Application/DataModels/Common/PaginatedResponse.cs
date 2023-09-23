using System.Collections.Immutable;
using Mapster;

namespace Application.DataModels.Common;

public class PaginatedResponse<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public int TotalCount { get; }

    public PaginatedResponse(IReadOnlyCollection<T> items, int totalCount)
    {
        TotalCount = totalCount;
        Items = items;
    }

    public static PaginatedResponse<T> Empty => new(ImmutableList<T>.Empty, 0);
    public static PaginatedResponse<T> From(IEnumerable<T> enumerable)
    {
        var elements = enumerable.ToList().AsReadOnly();
        return new PaginatedResponse<T>(elements, elements.Count);
    }
    
    public static PaginatedResponse<T> AdaptFrom<T2>(IEnumerable<T2> enumerable)
    {
        var elements = enumerable.Adapt<IEnumerable<T>>().ToList().AsReadOnly();
        return new PaginatedResponse<T>(elements, enumerable.Count());
    }
    
    public static PaginatedResponse<T> AdaptFromPaginatedResult<T2>(PaginatedResponse<T2> paginatedResponse)
    {
        var elements = paginatedResponse.Items.Adapt<IEnumerable<T>>().ToList().AsReadOnly();
        return new PaginatedResponse<T>(elements, paginatedResponse.TotalCount);
    }
}