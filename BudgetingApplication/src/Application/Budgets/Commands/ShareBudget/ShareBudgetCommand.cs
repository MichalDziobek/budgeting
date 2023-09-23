using MediatR;

namespace Application.Budgets.Commands.ShareBudget;

public class ShareBudgetCommand : IRequest
{
    public int BudgetId { get; set; }
    public string SharedUserId { get; set; } = string.Empty;
}