using Application.Budgets.Commands.ShareBudgetCommand;
using Application.Budgets.Commands.UpdateBudgetNameCommand;
using AutoFixture;
using FluentValidation.TestHelper;

namespace Application.Tests.Unit.Budgets.Commands.UpdateBudgetName;

public class ShareBudgetCommandValidatorTests
{
    private readonly ShareBudgetCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_OnCorrectData()
    {
        //Arrange
        var command = new ShareBudgetCommand()
        {
            SharedUserId = "UserId",
            BudgetId = 3
        };
        
        //Act
        var result = _sut.TestValidate(command);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
    
    [Fact]
    public void ShouldFail_OnEmptyBudgetId()
    {
        //Arrange
        var command = new ShareBudgetCommand()
        {
            SharedUserId = "UserId",
            BudgetId = 0
        };
        
        //Act
        var result = _sut.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.BudgetId);
    }
    
    [Fact]
    public void ShouldFail_OnEmptyUserId()
    {
        //Arrange
        var command = new ShareBudgetCommand()
        {
            SharedUserId = string.Empty,
            BudgetId = 3
        };
        
        //Act
        var result = _sut.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.SharedUserId);
    }
}