namespace Domain.Entities;

public class User : BaseEntity<string>
{
    public string FullName { get; set; } = string.Empty;

    public ICollection<Budget> OwnedBudgets { get; set; } = new List<Budget>();
}