namespace Application.BudgetEntries.DataModel;

public class BudgetEntryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
}