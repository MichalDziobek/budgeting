using Application.Budgets.Commands.UpdateBudgetNameCommand;
using AutoFixture;
using FluentValidation.TestHelper;

namespace Application.Tests.Unit.Budgets.Commands.UpdateBudgetName;

public class UpdateBudgetNameCommandValidatorTests
{
    private readonly UpdateBudgetNameCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_OnCorrectData()
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
    public void ShouldFail_OnEmptyName()
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
    public void ShouldFail_OnEmptyId()
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
    public void ShouldFail_OnTooLongName()
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