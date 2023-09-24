using Application.Budgets.Commands.UpdateBudgetName;
using AutoFixture;
using FluentValidation.TestHelper;

namespace Application.Tests.Unit.Budgets.Commands.UpdateBudgetName;

public class UpdateBudgetNameCommandValidatorTests
{
    private readonly UpdateBudgetNameCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCorrectData()
    {
        //Arrange
        var command = new UpdateBudgetNameCommand()
        {
            Name = "Budget Name",
            BudgetId = 3
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
        var command = new UpdateBudgetNameCommand()
        {
            Name = string.Empty,
            BudgetId = 3
        };
        
        //Act
        var result = _sut.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
    
    [Fact]
    public void ShouldFail_WhenEmptyId()
    {
        //Arrange
        var command = new UpdateBudgetNameCommand()
        {
            Name = "Budget Name",
            BudgetId = 0
        };
        
        //Act
        var result = _sut.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.BudgetId);
    }
    

    [Fact]
    public void ShouldFail_WhenTooLongName()
    {
        //Arrange
        var fixture = new Fixture();

        var command = new UpdateBudgetNameCommand
        {
            Name = string.Join("", fixture.CreateMany<char>(500))
        };

        //Act
        var result = _sut.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}