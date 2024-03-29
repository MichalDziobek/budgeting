using Application.Categories.Commands.CreateCategory;
using AutoFixture;
using FluentValidation.TestHelper;

namespace Application.Tests.Unit.Categories.Commands.CreateCategory;

public class CreateCategoryCommandValidatorTests
{
    private readonly CreateCategoryCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCorrectData()
    {
        //Arrange
        var command = new CreateCategoryCommand()
        {
            Name = "Category Name"
        };
        
        //Act
        var result = _sut.TestValidate(command);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
    
    [Fact]
    public void ShouldFail_WhenEmptyName()
    {
        //Arrange
        var command = new CreateCategoryCommand()
        {
            Name = string.Empty
        };
        
        //Act
        var result = _sut.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
    

    [Fact]
    public void ShouldFail_WhenTooLongName()
    {
        //Arrange
        var fixture = new Fixture();

        var command = new CreateCategoryCommand
        {
            Name = string.Join("", fixture.CreateMany<char>(500))
        };

        //Act
        var result = _sut.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}