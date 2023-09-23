namespace Domain.Entities;

public class BudgetEntryCategory : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public ICollection<BudgetEntry> BudgetEntries { get; set; } = new List<BudgetEntry>();
}