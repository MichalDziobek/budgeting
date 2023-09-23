using Domain.Enums;
using MediatR;

namespace Application.BudgetEntries.Queries.GetBudgetEntries;

public class GetBudgetEntriesQuery : IRequest<GetBudgetEntriesResponse>
{
    public int BudgetId { get; set; }
    public int Offset { get; set; } = 0;
    public int Limit { get; set; } = 25;
    public int? CategoryFilter { get; set; }
    public BudgetEntryType? BudgetEntryTypeFilter { get; set; }
}