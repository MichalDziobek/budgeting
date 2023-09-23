using System.Net;
using System.Net.Http.Json;
using Application.Abstractions;
using Application.Budgets.Commands.ShareBudgetCommand;
using AutoFixture;
using Domain.Entities;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using WebApi.Tests.Integration.Common;
using WebApi.Tests.Integration.Common.Abstractions;
using WebApi.Tests.Integration.Users;
using Xunit;

namespace WebApi.Tests.Integration.Budgets.Commands.UpdateBudgetName;

[Collection(nameof(SharedTestCollection))]
public class ShareBudgetTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITestDatabase _testDatabase;
    private readonly List<User> _initialUsers;

    private int _existingBudgetId;
    
    private string EndpointPath(int? id = null) => $"budgets/{id ?? _existingBudgetId}/share";

    public ShareBudgetTests(CustomWebApplicationFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
        _currentUserService = apiFactory.CurrentUserService;
        _testDatabase = apiFactory.GetTestDatabase();
        
        var fixture = new Fixture();
        _initialUsers = new List<User>()
        {
            new() { Id = fixture.Create<string>(), FullName = "John Doe", Email = "john.doe@example.com" },
            new() { Id = fixture.Create<string>(), FullName = "Jane Doe", Email = "jane.doe@example.com" },
        };
        
        _currentUserService.UserId.Returns(_initialUsers[0].Id);
        
    }

    public async Task InitializeAsync()
    {
        await _testDatabase.AddRangeAsync<User, string>(_initialUsers);
        
        var budget = BudgetsTestsData.DefaultBudget;
        budget.OwnerId = _initialUsers[0].Id;
        _existingBudgetId = (await _testDatabase.AddAsync<Budget, int>(budget)).Id;
    }

    public async Task DisposeAsync() => await _testDatabase.ResetAsync();

    [Fact]
    public async Task UpdateName_ShouldReturnOk_OnCorrectRequest()
    {
        //Arrange
        var command = CorrectShareBudgetCommand;
        
        //Act
        var response = await _client.PutAsJsonAsync(EndpointPath(), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task UpdateName_ShouldUpdateDb_OnCorrectRequest()
    {
        //Arrange
        var command = CorrectShareBudgetCommand;
        
        //Act
        var response = await _client.PutAsJsonAsync(EndpointPath(), command);
        var entity = await _testDatabase.FindAsync<Budget, int>(_existingBudgetId);

        //Assert
        entity.Should().NotBeNull();
        entity!.SharedBudgets.Should().Contain(x => x.UserId == _initialUsers[1].Id);
    }
    
    [Theory]
    [InlineData("")]
    public async Task UpdateName_ShouldReturnBadRequest_OnIncorrectRequestData(string userId)
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
    public async Task UpdateName_ShouldReturnUnauthorized_WhenUserIdIsEmpty()
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
    public async Task UpdateName_ShouldReturnNotFound_WhenBudgetDoesNotExist()
    {
        //Arrange
        var command = CorrectShareBudgetCommand;
        
        //Act
        var response = await _client.PutAsJsonAsync(EndpointPath(-1), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task UpdateName_ShouldReturnBadRequest_WhenUserDoesNotExist()
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
    public async Task UpdateName_ShouldReturnForbidden_OnOwnerIdNotMatchingCurrentUserId()
    {
        //Arrange
        var command = CorrectShareBudgetCommand;
        _currentUserService.UserId.Returns("DifferentUserId");
        
        //Act
        var response = await _client.PutAsJsonAsync(EndpointPath(), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private ShareBudgetCommand CorrectShareBudgetCommand => new ShareBudgetCommand
    {
        BudgetId = _existingBudgetId,
        SharedUserId = _initialUsers[1].Id
    };
}