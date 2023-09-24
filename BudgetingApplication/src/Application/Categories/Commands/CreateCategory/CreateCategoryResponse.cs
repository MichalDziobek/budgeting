using Application.Categories.DataModel;

namespace Application.Categories.Commands.CreateCategoryCommand;

public class CreateCategoryResponse
{
    public CategoryDto Category { get; set; } = default!;
}