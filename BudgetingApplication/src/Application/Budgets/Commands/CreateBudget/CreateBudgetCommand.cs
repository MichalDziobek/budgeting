using MediatR;

namespace Application.Budgets.Commands.CreateBudget;

public class CreateBudgetCommand : IRequest<CreateBudgetResponse>
{
    public string Name { get; set; } = string.Empty;
}