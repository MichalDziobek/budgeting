using MediatR;

namespace Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommand : IRequest<CreateCategoryResponse>
{
    public string Name { get; set; } = string.Empty;
}