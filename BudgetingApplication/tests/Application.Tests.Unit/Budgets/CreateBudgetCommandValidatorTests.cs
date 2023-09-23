using Application.Budgets.Commands.CreateBudget;
using AutoFixture;
using FluentValidation.TestHelper;

namespace Application.Tests.Unit.Budgets;

public class CreateBudgetCommandValidatorTests
{
    private readonly CreateBudgetCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_OnCorrectData()
    {
        //Arrange
        var command = new CreateBudgetCommand()
        {
            Name = "Budget Name"
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
        var command = new CreateBudgetCommand()
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

        var command = new CreateBudgetCommand
        {
            Name = string.Join("", fixture.CreateMany<char>(500))
        };

        //Act
        var result = _sut.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}