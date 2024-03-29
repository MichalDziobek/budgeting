using System.Net;
using System.Net.Http.Json;
using Application.Abstractions;
using Application.Budgets.Commands.ShareBudget;
using AutoFixture;
using Domain.Entities;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using WebApi.Tests.Integration.Common;
using WebApi.Tests.Integration.Common.Abstractions;
using Xunit;

namespace WebApi.Tests.Integration.Budgets.Commands.ShareBudget;

[Collection(nameof(SharedTestCollection))]
public class ShareBudgetTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITestDatabase _testDatabase;
    private List<User> _initialUsers = new();

    private int _existingBudgetId;
    
    private string EndpointPath(int? id = null) => $"budgets/{id ?? _existingBudgetId}/share";

    public ShareBudgetTests(CustomWebApplicationFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
        _currentUserService = apiFactory.CurrentUserService;
        _testDatabase = apiFactory.GetTestDatabase();
        
        PrepareData();

        _currentUserService.UserId.Returns(_initialUsers[0].Id);
        
    }


    public async Task InitializeAsync()
    {
        await _testDatabase.AddRangeAsync<User, string>(_initialUsers);
        
        var budget = BudgetsTestsData.DefaultEntity;
        budget.OwnerId = _initialUsers[0].Id;
        _existingBudgetId = (await _testDatabase.AddAsync<Budget, int>(budget)).Id;
    }

    public async Task DisposeAsync() => await _testDatabase.ResetAsync();

    [Fact]
    public async Task Share_ShouldReturnOk_WhenCorrectRequest()
    {
        //Arrange
        var command = CorrectShareBudgetCommand;
        
        //Act
        var response = await _client.PutAsJsonAsync(EndpointPath(), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Share_ShouldUpdateDb_WhenCorrectRequest()
    {
        //Arrange
        var command = CorrectShareBudgetCommand;

        //Act
        await _client.PutAsJsonAsync(EndpointPath(), command);

        var entity = await _testDatabase.FindAsync<Budget, int>(_existingBudgetId);

        //Assert
        entity.Should().NotBeNull();
        entity!.SharedBudgets.Should().Contain(x => x.UserId == _initialUsers[1].Id);
    }
    
    [Theory]
    [InlineData("")]
    public async Task Share_ShouldReturnBadRequest_WhenIncorrectRequestData(string userId)
    {
        //Arrange
        var command = CorrectShareBudgetCommand;
        command.SharedUserId = userId;
        
        //Act
        var response = await _client.PutAsJsonAsync(EndpointPath(), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Share_ShouldReturnUnauthorized_WhenUserIdIsEmpty()
    {
        //Arrange
        var command = CorrectShareBudgetCommand;
        _currentUserService.UserId.ReturnsNull();
        
        //Act
        var response = await _client.PutAsJsonAsync(EndpointPath(), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task Share_ShouldReturnNotFound_WhenBudgetDoesNotExist()
    {
        //Arrange
        var command = CorrectShareBudgetCommand;
        
        //Act
        var response = await _client.PutAsJsonAsync(EndpointPath(-1), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Share_ShouldReturnBadRequest_WhenUserDoesNotExist()
    {
        //Arrange
        var command = CorrectShareBudgetCommand;
        command.SharedUserId = "-1";
        
        //Act
        var response = await _client.PutAsJsonAsync(EndpointPath(), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Share_ShouldReturnForbidden_WhenOwnerIdNotMatchingCurrentUserId()
    {
        //Arrange
        var command = CorrectShareBudgetCommand;
        _currentUserService.UserId.Returns("DifferentUserId");
        
        //Act
        var response = await _client.PutAsJsonAsync(EndpointPath(), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private ShareBudgetCommand CorrectShareBudgetCommand => new()
    {
        BudgetId = _existingBudgetId,
        SharedUserId = _initialUsers[1].Id
    };
    
    private void PrepareData()
    {
        var fixture = new Fixture();
        _initialUsers = new List<User>()
        {
            new() { Id = fixture.Create<string>(), FullName = "John Doe", Email = "john.doe@example.com" },
            new() { Id = fixture.Create<string>(), FullName = "Jane Doe", Email = "jane.doe@example.com" },
        };
    }
}