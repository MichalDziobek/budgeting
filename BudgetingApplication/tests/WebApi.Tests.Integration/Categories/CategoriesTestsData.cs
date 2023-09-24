using Application.Categories.Commands.CreateCategoryCommand;
using Domain.Entities;

namespace WebApi.Tests.Integration.Categories;

public class CategoriesTestsData
{
    public const string DefaultName = "Category Name";

    public static CreateCategoryCommand CorrectCreateCommand => new()
    {
        Name = DefaultName,
    };

    // public static UpdateBudgetNameCommand CorrectUpdateCommand =>
    //     new UpdateBudgetNameCommand()
    //     {
    //         Name = DefaultName + "2",
    //     };

    public static Category DefaultEntity => new()
    {
        Name = DefaultName,
    };
}