using Application.Abstractions.Persistence;
using Application.Exceptions;
using MediatR;

namespace Application.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly ICategoriesRepository _categoriesRepository;

    public DeleteCategoryCommandHandler(ICategoriesRepository categoriesRepository)
    {
        _categoriesRepository = categoriesRepository;
    }

    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoriesRepository.GetById(request.CategoryId, cancellationToken);
        if (category is null)
        {
            throw new BadRequestException("Category requested for deletion does not exist");
        }


        await _categoriesRepository.Delete(category, cancellationToken);
    }
}