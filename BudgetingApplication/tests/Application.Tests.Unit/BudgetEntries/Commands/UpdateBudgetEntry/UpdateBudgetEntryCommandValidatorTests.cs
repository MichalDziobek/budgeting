using Application.BudgetEntries.Commands.UpdateBudgetEntry;
using AutoFixture;
using FluentValidation.TestHelper;

namespace Application.Tests.Unit.BudgetEntries.Commands.UpdateBudgetEntry;

public class UpdateBudgetEntryCommandValidatorTests
{
    private readonly UpdateBudgetEntryCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_OnCorrectData()
    {
        //Arrange
        var command = new UpdateBudgetEntryCommand
        {
            Name = "BudgetEntry Name",
            BudgetEntryId = 1,
            CategoryId = 1,

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
        var command = new UpdateBudgetEntryCommand()
        {
            Name = string.Empty
        };
        
        //Act
        var result = _sut.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
    
    [Fact]
    public void ShouldFail_OnEmptyBudgetId()
    {
        //Arrange
        var command = new UpdateBudgetEntryCommand()
        {
            BudgetEntryId = default
        };
        
        //Act
        var result = _sut.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.BudgetEntryId);
    }
    
    [Fact]
    public void ShouldFail_OnEmptyCategoryId()
    {
        //Arrange
        var command = new UpdateBudgetEntryCommand()
        {
            CategoryId = default
        };
        
        //Act
        var result = _sut.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public void ShouldFail_OnTooLongName()
    {
        //Arrange
        var fixture = new Fixture();

        var command = new UpdateBudgetEntryCommand
        {
            Name = string.Join("", fixture.CreateMany<char>(500))
        };

        //Act
        var result = _sut.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}