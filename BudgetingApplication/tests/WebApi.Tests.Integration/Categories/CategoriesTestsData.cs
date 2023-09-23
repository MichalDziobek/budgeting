using Application.Categories.Commands.CreateCategoryCommand;
using Domain.Entities;
using WebApi.Tests.Integration.Users;

namespace WebApi.Tests.Integration.Categories;

public class CategoriesTestsData
{
    public const string DefaultName = "Category Name";

    public static CreateCategoryCommand CorrectCreateCommand => new CreateCategoryCommand()
    {
        Name = DefaultName,
    };

    // public static UpdateBudgetNameCommand CorrectUpdateNameCommand =>
    //     new UpdateBudgetNameCommand()
    //     {
    //         Name = DefaultName + "2",
    //     };

    public static Category DefaultEntity => new Category()
    {
        Name = DefaultName,
    };
}