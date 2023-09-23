namespace Domain.Entities;

public class User : BaseEntity<string>
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public ICollection<Budget> OwnedBudgets { get; set; } = new List<Budget>();
    public ICollection<Budget> BudgetsSharedWithThisUser { get; set; } = new List<Budget>();
    public ICollection<SharedBudget> SharedBudgets { get; set; } = new List<SharedBudget>();

}