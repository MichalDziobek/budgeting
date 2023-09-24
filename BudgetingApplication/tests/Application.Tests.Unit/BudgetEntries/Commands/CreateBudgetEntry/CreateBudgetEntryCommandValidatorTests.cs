using Application.BudgetEntries.Commands.CreateBudgetEntry;
using AutoFixture;
using FluentValidation.TestHelper;

namespace Application.Tests.Unit.BudgetEntries.Commands.CreateBudgetEntry;

public class CreateBudgetEntryCommandValidatorTests
{
    private readonly CreateBudgetEntryCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCorrectData()
    {
        //Arrange
        var command = new CreateBudgetEntryCommand
        {
            Name = "BudgetEntry Name",
            BudgetId = 1,
            CategoryId = 1,

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
        var command = new CreateBudgetEntryCommand()
        {
            Name = string.Empty
        };
        
        //Act
        var result = _sut.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
    
    [Fact]
    public void ShouldFail_WhenEmptyBudgetId()
    {
        //Arrange
        var command = new CreateBudgetEntryCommand()
        {
            BudgetId = default
        };
        
        //Act
        var result = _sut.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.BudgetId);
    }
    
    [Fact]
    public void ShouldFail_WhenEmptyCategoryId()
    {
        //Arrange
        var command = new CreateBudgetEntryCommand()
        {
            CategoryId = default
        };
        
        //Act
        var result = _sut.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public void ShouldFail_WhenTooLongName()
    {
        //Arrange
        var fixture = new Fixture();

        var command = new CreateBudgetEntryCommand
        {
            Name = string.Join("", fixture.CreateMany<char>(500))
        };

        //Act
        var result = _sut.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}