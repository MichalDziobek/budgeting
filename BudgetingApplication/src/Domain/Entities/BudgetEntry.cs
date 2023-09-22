namespace Domain.Entities;

public class BudgetEntry : BaseEntity<int>
{
    public string? Name { get; set; }
    public decimal Value { get; set; }
    public BudgetEntryType Type { get; set; }

    public int BudgetId { get; set; }
    public Budget Budget { get; set; } = default!;
}