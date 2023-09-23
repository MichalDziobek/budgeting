namespace Application.Budgets.DataModels;

public class BudgetDto
{
    public int Id { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}