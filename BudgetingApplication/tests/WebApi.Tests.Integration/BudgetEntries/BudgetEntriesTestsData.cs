using Application.BudgetEntries.Commands.CreateBudgetEntry;
using Domain.Entities;

namespace WebApi.Tests.Integration.BudgetEntries;

public class BudgetEntriesTestsData
{
    public const string DefaultName = "Budget Entry Name";
    public const decimal DefaultValue = 100.50M;

    public static CreateBudgetEntryCommand CorrectCreateCommand(int budgetId, int categoryId) => new ()
    {
        Name = DefaultName,
        CategoryId = categoryId,
        BudgetId = budgetId,
        Value = DefaultValue,
    };

    // public static UpdateBudgetNameCommand CorrectUpdateNameCommand =>
    //     new UpdateBudgetNameCommand()
    //     {
    //         Name = DefaultName + "2",
    //     };

    public static BudgetEntry DefaultEntity(int budgetId, int categoryId) => new ()
    {
        Name = DefaultName,
        BudgetId = budgetId,
        CategoryId = categoryId,
        Value = DefaultValue
    };
}