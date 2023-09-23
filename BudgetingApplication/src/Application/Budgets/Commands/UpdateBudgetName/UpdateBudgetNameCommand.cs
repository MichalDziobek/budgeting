using MediatR;

namespace Application.Budgets.Commands.UpdateBudgetNameCommand;

public class UpdateBudgetNameCommand : IRequest
{
    public int BudgetId { get; set; }
    public string Name { get; set; } = string.Empty;
}