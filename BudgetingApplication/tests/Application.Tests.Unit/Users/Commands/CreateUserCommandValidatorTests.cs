using Application.Users.Commands.CreateUser;
using AutoFixture;
using FluentValidation.TestHelper;

namespace Application.Tests.Unit.Users.Commands;

public class CreateUserCommandValidatorTests
{
    private readonly CreateUserCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCorrectData()
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
    public void ShouldFail_WhenIncorrectEmail(string email)
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
    public void ShouldFail_WhenTooLongEmail()
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
    public void ShouldFail_WhenTooLongName()
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
}