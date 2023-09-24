using Domain.Events;

namespace Domain.Entities;

public class BudgetEntry : BaseEntity<int>
{
    public string? Name { get; set; }

    private decimal _value;
    public decimal Value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                AddDomainEvent(new BudgetEntryChangedValueEvent(_value, this));
            }
            _value = value;
        }
    }

    public int BudgetId { get; set; }
    public Budget Budget { get; set; } = default!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = default!;
}