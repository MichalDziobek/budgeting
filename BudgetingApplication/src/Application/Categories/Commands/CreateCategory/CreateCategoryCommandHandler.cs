using Application.Abstractions.Persistence;
using Application.Categories.DataModel;
using Application.Exceptions;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CreateCategoryResponse>
{
    private readonly ICategoriesRepository _categoriesRepository;

    public CreateCategoryCommandHandler(ICategoriesRepository categoriesRepository)
    {
        _categoriesRepository = categoriesRepository;
    }

    public async Task<CreateCategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (await _categoriesRepository.Exists(x => x.Name == request.Name, cancellationToken))
        {
            throw new BadRequestException("This category already exists");
        }

        var category = request.Adapt<Category>();

        var createdCategory = await _categoriesRepository.Create(category, cancellationToken);
        var response = new CreateCategoryResponse()
        {
            Category = createdCategory.Adapt<CategoryDto>()
        };

        return response;
    }
}