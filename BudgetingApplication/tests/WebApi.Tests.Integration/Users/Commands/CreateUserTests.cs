using System.Net;
using System.Net.Http.Json;
using Application.Abstractions;
using Application.Users.Commands;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace WebApi.Tests.Integration.Users.Commands;

[Collection(nameof(SharedTestCollection))]
public class CreateUserTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly Func<Task> _resetDb;
    private readonly ICurrentUserService _currentUserService;

    public CreateUserTests(CustomWebApplicationFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
        _currentUserService = apiFactory.CurrentUserService;
        _resetDb = apiFactory.ResetDatabaseAsync;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDb();

    [Fact]
    public async Task Create_ShouldReturnOkObjectResult_OnCorrectRequest()
    {
        //Arrange
        var command = new CreateUserCommand
        {
            Email = "correct@email.com",
            FullName = "fullName"
        };
        _currentUserService.UserId.Returns("userId");
        
        //Act
        var response = await _client.PostAsJsonAsync("users", command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    //TODO More tests here
}