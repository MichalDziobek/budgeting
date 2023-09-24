using Application.Budgets.Commands.ShareBudget;
using FluentValidation.TestHelper;

namespace Application.Tests.Unit.Budgets.Commands.ShareBudget;

public class ShareBudgetCommandValidatorTests
{
    private readonly ShareBudgetCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCorrectData()
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
    public void ShouldFail_WhenEmptyBudgetId()
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
    public void ShouldFail_WhenEmptyUserId()
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