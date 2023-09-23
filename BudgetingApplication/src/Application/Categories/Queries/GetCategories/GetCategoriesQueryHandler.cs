using Application.Abstractions.Persistence;
using Application.Categories.DataModel;
using Domain.Entities;
using Mapster;
using MediatR;

namespace Application.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, GetCategoriesResponse>
{
    private readonly ICategoriesRepository _categoriesRepository;

    public GetCategoriesQueryHandler(ICategoriesRepository categoriesRepository)
    {
        _categoriesRepository = categoriesRepository;
    }

    public async Task<GetCategoriesResponse> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var filters = GetFilters(request);

        var categoryDtos = (await _categoriesRepository.GetCollection(filters, cancellationToken)).Adapt<IEnumerable<CategoryDto>>();
        var response = new GetCategoriesResponse()
        {
            Categories = categoryDtos
        };
        return response;
    }

    private static Func<IQueryable<Category>, IQueryable<Category>> GetFilters(GetCategoriesQuery request)
    {
        Func<IQueryable<Category>, IQueryable<Category>> filters = users => users;
        if (request.NameSearchQuery is not null)
        {
            filters = users => users.Where(user => user.Name.Contains(request.NameSearchQuery));
        }

        return filters;
    }
}