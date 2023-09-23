using MediatR;

namespace Application.BudgetEntries.Commands.CreateBudgetEntry;

public class CreateBudgetEntryCommand : IRequest<CreateBudgetEntryResponse>
{
    public string Name { get; set; } = string.Empty;
    public int BudgetId { get; set; }
    public int CategoryId { get; set; }
    public decimal Value { get; set; }
}