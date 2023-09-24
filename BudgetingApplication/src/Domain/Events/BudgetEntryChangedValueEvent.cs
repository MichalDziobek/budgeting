using Domain.Entities;
using MediatR;

namespace Domain.Events;

public class BudgetEntryChangedValueEvent : INotification
{
    public decimal OldValue { get; set; }
    public BudgetEntry BudgetEntry { get; set; }

    public BudgetEntryChangedValueEvent(decimal oldValue, BudgetEntry budgetEntry)
    {
        OldValue = oldValue;
        BudgetEntry = budgetEntry;
    }
}