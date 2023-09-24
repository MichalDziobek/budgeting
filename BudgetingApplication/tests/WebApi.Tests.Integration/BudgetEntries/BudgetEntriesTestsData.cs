using Application.BudgetEntries.Commands.CreateBudgetEntry;
using Application.BudgetEntries.Commands.UpdateBudgetEntry;
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

    public static UpdateBudgetEntryCommand CorrectUpdateCommand(int budgetEntryId, int categoryId) =>
        new ()
        {
            BudgetEntryId = budgetEntryId,
            Name = DefaultName + "2",
            CategoryId = categoryId,
            Value = 100,
        };

    public static BudgetEntry DefaultEntity(int budgetId, int categoryId) => new ()
    {
        Name = DefaultName,
        BudgetId = budgetId,
        CategoryId = categoryId,
        Value = DefaultValue
    };
}