namespace Domain.Entities;

public class Budget : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    
    public string OwnerId = string.Empty;
    public User Owner { get; set; } = default!;

    public ICollection<User> UsersWithSharedAccess { get; set; } = new List<User>();
    public ICollection<SharedBudget> SharedBudgets { get; set; } = new List<SharedBudget>();

    public ICollection<BudgetEntry> BudgetEntries { get; set; } = new List<BudgetEntry>();
}

public class SharedBudget
{
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; }  = default!;
    public int BudgetId { get; set; }
    public Budget Budget { get; set; } = default!;
}