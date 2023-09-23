using System.Net;
using System.Net.Http.Json;
using Application.Abstractions;
using Application.Users.Commands;
using Domain.Entities;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using WebApi.Tests.Integration.Common;
using WebApi.Tests.Integration.Common.Abstractions;
using Xunit;

namespace WebApi.Tests.Integration.Users.Commands;

public class UserTestsData
{
    public const string CorrectEmail = "correct-email@example.com";
    public const string CorrectName = "Jane Doe";
    public const string DefaultUserId = "auth0|12345";

    public static CreateUserCommand CorrectCreateUserCommand => new CreateUserCommand
    {
        Email = CorrectEmail,
        FullName = CorrectName
    };

    public static User DefaultUser => new User
    {
        Id = DefaultUserId,
        FullName = CorrectName,
        Email = CorrectEmail,
    };
}

[Collection(nameof(SharedTestCollection))]
public class CreateUserTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITestDatabase _testDatabase;
    private readonly Func<Task> _resetDb;
    private readonly Func<string, Task<User?>> _findUser;

    public CreateUserTests(CustomWebApplicationFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
        _currentUserService = apiFactory.CurrentUserService;
        _testDatabase = apiFactory.GetTestRepository();
        // _resetDb = apiFactory.ResetDatabaseAsync;
        // _findUser = apiFactory.FindAsync<User, string>;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _testDatabase.ResetAsync();

    [Fact]
    public async Task Create_ShouldReturnOk_OnCorrectRequest()
    {
        //Arrange
        var command = UserTestsData.CorrectCreateUserCommand;
        _currentUserService.UserId.Returns(UserTestsData.DefaultUserId);
        
        //Act
        var response = await _client.PostAsJsonAsync("users", command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Create_ShouldAddToDb_OnCorrectRequest()
    {
        //Arrange
        var command = UserTestsData.CorrectCreateUserCommand;
        _currentUserService.UserId.Returns(UserTestsData.DefaultUserId);
        var expected = UserTestsData.DefaultUser;
        
        //Act
        _ = await _client.PostAsJsonAsync("users", command);
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
        var response = await _client.PostAsJsonAsync("users", command);
        
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
        var response = await _client.PostAsJsonAsync("users", command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenUserAlreadyExists()
    {
        //Arrange
        var command = UserTestsData.CorrectCreateUserCommand;
        await _testDatabase.AddAsync(UserTestsData.DefaultUser);
        _currentUserService.UserId.Returns(UserTestsData.DefaultUserId);
        
        //Act
        var response = await _client.PostAsJsonAsync("users", command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}