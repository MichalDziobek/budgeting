using Application.Categories.DataModel;

namespace Application.Categories.Queries.GetCategories;

public class GetCategoriesResponse
{
    public IEnumerable<CategoryDto> Categories { get; set; } = Enumerable.Empty<CategoryDto>();
}