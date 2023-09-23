using Application.Budgets.DataModels;

namespace Application.Budgets.Commands.CreateBudget;

public class CreateBudgetResponse
{
    public BudgetDto Budget { get; set; } = default!;
}