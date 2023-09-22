using Application.Users.Commands;
using AutoFixture;
using FluentValidation.TestHelper;

namespace Application.Tests.Unit.Users.Command;

public class CreateUserCommandValidatorTests
{
    private readonly CreateUserCommandValidator _sut;

    public CreateUserCommandValidatorTests()
    {
        _sut = new CreateUserCommandValidator();
    }

    [Fact]
    public void ShouldPass_OnCorrectData()
    {
        //Arrange
        var command = new CreateUserCommand
        {
            Email = "correct@email.com",
            FullName = "Full Name"
        };
        
        //Act
        var result = _sut.TestValidate(command);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
    
    [Theory]
    [InlineData("foo")]
    [InlineData("")]
    [InlineData("incorrect@@@email.com")]
    public void ShouldFail_OnIncorrectEmail(string email)
    {
        //Arrange
        var command = new CreateUserCommand
        {
            Email = email,
            FullName = "Full Name"
        };
        
        //Act
        var result = _sut.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
    
    [Fact]
    public void ShouldFail_OnTooLongEmail()
    {
        //Arrange
        var fixture = new Fixture();
        
        var command = new CreateUserCommand
        {
            Email = string.Join("", fixture.CreateMany<char>(500)),
            FullName = "Full Name"
        };
        
        //Act
        var result = _sut.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void ShouldFail_OnTooLongName()
    {
        //Arrange
        var fixture = new Fixture();

        var command = new CreateUserCommand
        {
            Email = "correct@email.com",
            FullName = string.Join("", fixture.CreateMany<char>(500))
        };

        //Act
        var result = _sut.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }
    
    [Fact]
    public void ShouldFail_OnEmptyName()
    {
        //Arrange
        var fixture = new Fixture();

        var command = new CreateUserCommand
        {
            Email = "correct@email.com",
            FullName = string.Empty
        };

        //Act
        var result = _sut.TestValidate(command);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.FullName);
    }
}