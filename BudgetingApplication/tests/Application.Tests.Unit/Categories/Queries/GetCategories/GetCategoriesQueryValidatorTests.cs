using Application.Categories.Queries.GetCategories;
using AutoFixture;
using FluentValidation.TestHelper;

namespace Application.Tests.Unit.Categories.Queries.GetCategories;

public class GetCategoriesQueryValidatorTests
{
    private readonly GetCategoriesQueryValidator _sut = new();

    [Fact]
    public void ShouldPass_OnEmptyData()
    {
        //Arrange
        var query = new GetCategoriesQuery();
        
        //Act
        var result = _sut.TestValidate(query);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    
    [Fact]
    public void ShouldFail_OnTooLongName()
    {
        //Arrange
        var fixture = new Fixture();
        
        var query = new GetCategoriesQuery()
        {
            NameSearchQuery = string.Join("", fixture.CreateMany<char>(500)),
        };
        
        //Act
        var result = _sut.TestValidate(query);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.NameSearchQuery);
    }
}