using MediatR;

namespace Application.Budgets.Commands.UpdateBudgetName;

public class UpdateBudgetNameCommand : IRequest
{
    public int BudgetId { get; set; }
    public string Name { get; set; } = string.Empty;
}