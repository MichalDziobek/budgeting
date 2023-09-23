using MediatR;

namespace Application.Categories.Queries.GetCategories;

public class GetCategoriesQuery : IRequest<GetCategoriesResponse>
{
    public string? NameSearchQuery { get; set; }
}