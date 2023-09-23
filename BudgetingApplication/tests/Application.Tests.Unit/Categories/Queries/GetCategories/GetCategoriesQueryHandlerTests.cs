using Application.Abstractions.Persistence;
using Application.Categories.DataModel;
using Application.Categories.Queries.GetCategories;
using Application.Tests.Unit.Extensions;
using AutoFixture;
using Domain.Entities;
using Mapster;

namespace Application.Tests.Unit.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandlerTests
{
    private readonly GetCategoriesQueryHandler _sut;
    private readonly List<Category> _users;

    public GetCategoriesQueryHandlerTests()
    {
        var categoriesRepository = Substitute.For<ICategoriesRepository>();

        var fixture = new Fixture().ChangeToOmitOnRecursionBehaviour();
        _users = fixture.CreateMany<Category>(5).ToList();
        
        categoriesRepository.MockGetCollection(_users);
        
        _sut = new GetCategoriesQueryHandler(categoriesRepository);
    }

    [Fact]
    public async Task ShouldReturnAllResults_OnEmptyQuery()
    {
        //Arrange
        var expectedCategories = _users.Adapt<IEnumerable<CategoryDto>>();
        var expectedResponse = new GetCategoriesResponse()
        {
            Categories = expectedCategories
        };
        var query = new GetCategoriesQuery();

        //Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        //Assert
        result.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    public async Task ShouldReturnFilteredCategories_OnQueryWithNameFilter(int userIndex)
    {
        //Arrange
        var fullName = _users[userIndex].Name;
        var nameSearchQuery = fullName[..(fullName.Length/2)];
        var expectedCategories = _users.Where(x => x.Name.Contains(nameSearchQuery)).Adapt<IEnumerable<CategoryDto>>();
        var expectedResponse = new GetCategoriesResponse()
        {
            Categories = expectedCategories
        };
        var query = new GetCategoriesQuery()
        {
            NameSearchQuery = nameSearchQuery
        };

        //Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        //Assert
        result.Should().BeEquivalentTo(expectedResponse);
    }
}