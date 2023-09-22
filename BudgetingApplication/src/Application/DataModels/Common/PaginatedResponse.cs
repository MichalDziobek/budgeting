namespace Application.DataModels.Common;

public class PaginatedResponse<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public int TotalTotalCount { get; }

    public PaginatedResponse(IReadOnlyCollection<T> items, int totalCount)
    {
        TotalTotalCount = totalCount;
        Items = items;
    }
}