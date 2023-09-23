using Application.Budgets.Commands.CreateBudget;
using Domain.Entities;
using WebApi.Tests.Integration.Users;

namespace WebApi.Tests.Integration.Budgets;

public class BudgetsTestsData
{
    public const string DefaultName = "Budget Name";

    public static CreateBudgetCommand CorrectCreateBudgetCommand => new CreateBudgetCommand()
    {
        Name = DefaultName,
    };

    public static Budget DefaultBudget => new Budget()
    {
        OwnerId = UserTestsData.DefaultUserId,
        Name = DefaultName,
    };
}