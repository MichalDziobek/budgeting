using System.Net;
using System.Net.Http.Json;
using Application.Abstractions;
using Application.Users.Commands.CreateUser;
using Domain.Entities;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using WebApi.Tests.Integration.Common;
using WebApi.Tests.Integration.Common.Abstractions;
using Xunit;

namespace WebApi.Tests.Integration.Users.Commands;

[Collection(nameof(SharedTestCollection))]
public class CreateUserTests : IAsyncLifetime
{
    private const string PathPrefix = "users";

    private readonly HttpClient _client;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITestDatabase _testDatabase;

    public CreateUserTests(CustomWebApplicationFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
        _currentUserService = apiFactory.CurrentUserService;
        _testDatabase = apiFactory.GetTestDatabase();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _testDatabase.ResetAsync();

    [Fact]
    public async Task Create_ShouldReturnOk_WhenCorrectRequest()
    {
        //Arrange
        var command = UserTestsData.CorrectCreateUserCommand;
        _currentUserService.UserId.Returns(UserTestsData.DefaultUserId);
        
        //Act
        var response = await _client.PostAsJsonAsync(PathPrefix, command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Create_ShouldAddToDb_WhenCorrectRequest()
    {
        //Arrange
        var command = UserTestsData.CorrectCreateUserCommand;
        _currentUserService.UserId.Returns(UserTestsData.DefaultUserId);
        var expected = UserTestsData.DefaultUser;
        
        //Act
        _ = await _client.PostAsJsonAsync(PathPrefix, command);
        var entity = await _testDatabase.FindAsync<User, string>(UserTestsData.DefaultUserId);
        
        //Assert
        entity.Should().BeEquivalentTo(expected);
    }
    
    [Theory]
    [InlineData("", "fullname")]
    [InlineData("correct@email.com", "")]
    [InlineData("incorrect@@email.com", "fullname")]
    [InlineData("", "")]
    public async Task Create_ShouldReturnBadRequest_IncorrectRequestData(string email, string fullname)
    {
        //Arrange
        var command = new CreateUserCommand
        {
            Email = email,
            FullName = fullname
        };
        _currentUserService.UserId.Returns(UserTestsData.DefaultUserId);
        
        //Act
        var response = await _client.PostAsJsonAsync(PathPrefix, command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Create_ShouldReturnUnauthorized_WhenUserIdIsEmpty()
    {
        //Arrange
        var command = UserTestsData.CorrectCreateUserCommand;
        _currentUserService.UserId.ReturnsNull();
        
        //Act
        var response = await _client.PostAsJsonAsync(PathPrefix, command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenUserAlreadyExists()
    {
        //Arrange
        var command = UserTestsData.CorrectCreateUserCommand;
        await _testDatabase.AddAsync<User, string>(UserTestsData.DefaultUser);
        _currentUserService.UserId.Returns(UserTestsData.DefaultUserId);
        
        //Act
        var response = await _client.PostAsJsonAsync(PathPrefix, command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}