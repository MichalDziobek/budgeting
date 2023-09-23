using Application.Users.Queries.GetUsers;
using AutoFixture;
using FluentValidation.TestHelper;

namespace Application.Tests.Unit.Users.Queries.GetUsers;

public class GetUsersQueryValidatorTests
{
    private readonly GetUsersQueryValidator _sut;

    public GetUsersQueryValidatorTests()
    {
        _sut = new GetUsersQueryValidator();
    }

    [Fact]
    public void ShouldPass_OnEmptyData()
    {
        //Arrange
        var query = new GetUsersQuery();
        
        //Act
        var result = _sut.TestValidate(query);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
    
    [Theory]
    [InlineData("foo")]
    [InlineData("")]
    [InlineData("incorrect@@@email.com")]
    public void ShouldPass_OnEmail(string email)
    {
        //Arrange
        var query = new GetUsersQuery()
        {
            EmailSearchQuery = email
        };
        
        //Act
        var result = _sut.TestValidate(query);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
    
    [Theory]
    [InlineData("foo")]
    [InlineData("")]
    [InlineData("John Doe")]
    public void ShouldPass_OnName(string name)
    {
        //Arrange
        var query = new GetUsersQuery()
        {
            FullNameSearchQuery = name
        };
        
        //Act
        var result = _sut.TestValidate(query);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
    
    [Fact]
    public void ShouldFail_OnTooLongEmail()
    {
        //Arrange
        var fixture = new Fixture();
        
        var query = new GetUsersQuery()
        {
            EmailSearchQuery = string.Join("", fixture.CreateMany<char>(500)),
        };
        
        //Act
        var result = _sut.TestValidate(query);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.EmailSearchQuery);
    }
    
    [Fact]
    public void ShouldFail_OnTooLongName()
    {
        //Arrange
        var fixture = new Fixture();
        
        var query = new GetUsersQuery()
        {
            FullNameSearchQuery = string.Join("", fixture.CreateMany<char>(500)),
        };
        
        //Act
        var result = _sut.TestValidate(query);

        //Assert
        result.ShouldHaveValidationErrorFor(x => x.FullNameSearchQuery);
    }

}