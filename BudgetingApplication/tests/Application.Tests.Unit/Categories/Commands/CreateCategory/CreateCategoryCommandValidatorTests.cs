using Application.Categories.Commands.CreateCategoryCommand;
using AutoFixture;
using FluentValidation.TestHelper;

namespace Application.Tests.Unit.Categories.Commands.CreateCategory;

public class CreateCategoryCommandValidatorTests
{
    private readonly CreateCategoryCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_OnCorrectData()
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
    public void ShouldFail_OnEmptyName()
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
    public void ShouldFail_OnTooLongName()
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