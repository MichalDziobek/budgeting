namespace Domain.Entities;

public class Budget : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    
    public string OwnerId = string.Empty;

    public decimal TotalValue { get; set; }
    
    public User Owner { get; set; } = default!;

    public ICollection<SharedBudget> SharedBudgets { get; set; } = new List<SharedBudget>();

    public ICollection<BudgetEntry> BudgetEntries { get; set; } = new List<BudgetEntry>();
}