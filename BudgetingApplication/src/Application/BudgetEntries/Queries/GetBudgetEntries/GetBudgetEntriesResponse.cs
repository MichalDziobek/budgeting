using Application.BudgetEntries.DataModel;
using Application.DataModels.Common;

namespace Application.BudgetEntries.Queries.GetBudgetEntries;

public class GetBudgetEntriesResponse
{
    public PaginatedResponse<BudgetEntryDto> BudgetEntries { get; set; } = PaginatedResponse<BudgetEntryDto>.Empty;
}