using MediatR;

namespace Application.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommand : IRequest
{
    public int CategoryId { get; set; }
}