using Application.Budgets.Commands.CreateBudget;
using Application.Budgets.Commands.UpdateBudgetName;
using Domain.Entities;
using WebApi.Tests.Integration.Users;

namespace WebApi.Tests.Integration.Budgets;

public class BudgetsTestsData
{
    public const string DefaultName = "Budget Name";

    public static CreateBudgetCommand CorrectCreateCommand => new()
    {
        Name = DefaultName,
    };

    public static UpdateBudgetNameCommand CorrectUpdateNameCommand =>
        new()
        {
            Name = DefaultName + "2",
        };

    public static Budget DefaultEntity => new()
    {
        OwnerId = UserTestsData.DefaultUserId,
        Name = DefaultName,
    };
}