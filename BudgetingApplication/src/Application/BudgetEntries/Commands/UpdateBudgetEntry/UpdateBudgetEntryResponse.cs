using Application.BudgetEntries.DataModel;

namespace Application.BudgetEntries.Commands.UpdateBudgetEntry;

public class UpdateBudgetEntryResponse
{
    public BudgetEntryDto BudgetEntry { get; set; } = default!;
}