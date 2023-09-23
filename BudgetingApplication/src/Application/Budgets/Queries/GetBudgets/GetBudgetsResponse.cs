using Application.Budgets.DataModels;

namespace Application.Budgets.Queries.GetBudgets;

public class GetBudgetsResponse
{
    public IEnumerable<BudgetDto> OwnedBudgets { get; set; } = Enumerable.Empty<BudgetDto>();
    public IEnumerable<BudgetDto> SharedBudgets { get; set; } = Enumerable.Empty<BudgetDto>();
}