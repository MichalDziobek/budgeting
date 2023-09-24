using Application.Categories.DataModel;

namespace Application.Categories.Commands.CreateCategory;

public class CreateCategoryResponse
{
    public CategoryDto Category { get; set; } = default!;
}