using MediatR;

namespace Application.BudgetEntries.Commands.UpdateBudgetEntry;

public class UpdateBudgetEntryCommand : IRequest<UpdateBudgetEntryResponse>
{
    public int BudgetEntryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public decimal Value { get; set; }
}