using MediatR;

namespace Application.Categories.Commands.CreateCategoryCommand;

public class CreateCategoryCommand : IRequest<CreateCategoryResponse>
{
    public string Name { get; set; } = string.Empty;
}