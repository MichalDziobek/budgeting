namespace Domain.Entities;

public class Budget : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    
    public string OwnerId = string.Empty;
    public User Owner { get; set; } = default!;

    public ICollection<User> UsersWithSharedAccess { get; set; } = new List<User>();

    public ICollection<BudgetEntry> BudgetEntries { get; set; } = new List<BudgetEntry>();
}