namespace Domain.Entities;

public class SharedBudget
{
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; }  = default!;
    public int BudgetId { get; set; }
    public Budget Budget { get; set; } = default!;
}