using Application.BudgetEntries.DataModel;

namespace Application.BudgetEntries.Commands.CreateBudgetEntry;

public class CreateBudgetEntryResponse
{
    public BudgetEntryDto BudgetEntry { get; set; } = default!;
}