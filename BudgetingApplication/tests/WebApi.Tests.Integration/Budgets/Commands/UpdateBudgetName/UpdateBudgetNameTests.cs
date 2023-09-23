using System.Net;
using System.Net.Http.Json;
using Application.Abstractions;
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
public class UpdateBudgetNameTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITestDatabase _testDatabase;
    private int _existingBudgetId;
    
    private string EndpointPath(int? id = null) => $"budgets/{id ?? _existingBudgetId}";

    public UpdateBudgetNameTests(CustomWebApplicationFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
        _currentUserService = apiFactory.CurrentUserService;
        _testDatabase = apiFactory.GetTestDatabase();
        
        _currentUserService.UserId.Returns(UserTestsData.DefaultUserId);
        
    }

    public async Task InitializeAsync()
    {
        await _testDatabase.AddAsync<User, string>(UserTestsData.DefaultUser);
        var budget = BudgetsTestsData.DefaultBudget;
        budget.OwnerId = UserTestsData.DefaultUserId;
        _existingBudgetId = (await _testDatabase.AddAsync<Budget, int>(budget)).Id;
    }

    public async Task DisposeAsync() => await _testDatabase.ResetAsync();

    [Fact]
    public async Task UpdateName_ShouldReturnOk_OnCorrectRequest()
    {
        //Arrange
        var command = BudgetsTestsData.CorrectUpdateNameCommand;
        
        //Act
        var response = await _client.PatchAsJsonAsync(EndpointPath(), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task UpdateName_ShouldUpdateDb_OnCorrectRequest()
    {
        //Arrange
        var command = BudgetsTestsData.CorrectUpdateNameCommand;
        var expected = await _testDatabase.FindAsync<Budget, int>(_existingBudgetId);
        expected!.Name = command.Name;
        
        //Act
        var response = await _client.PatchAsJsonAsync(EndpointPath(), command);
        var entity = await _testDatabase.FindAsync<Budget, int>(_existingBudgetId);

        
        //Assert
        entity.Should().BeEquivalentTo(expected);
    }
    
    [Theory]
    [InlineData("")]
    public async Task UpdateName_ShouldReturnBadRequest_IncorrectRequestData(string name)
    {
        //Arrange
        var command = BudgetsTestsData.CorrectUpdateNameCommand;
        command.Name = name;
        
        //Act
        var response = await _client.PatchAsJsonAsync(EndpointPath(), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task UpdateName_ShouldReturnUnauthorized_WhenUserIdIsEmpty()
    {
        //Arrange
        var command = BudgetsTestsData.CorrectUpdateNameCommand;
        _currentUserService.UserId.ReturnsNull();
        
        //Act
        var response = await _client.PatchAsJsonAsync(EndpointPath(), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task UpdateName_ShouldNotFound_WhenBudgetDoesNotExist()
    {
        //Arrange
        var command = BudgetsTestsData.CorrectUpdateNameCommand;
        // await _testDatabase.AddAsync<Budget, int>(BudgetsTestsData.DefaultBudget);
        
        //Act
        var response = await _client.PatchAsJsonAsync(EndpointPath(-1), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task UpdateName_ShouldReturnForbidden_OnOwnerIdNotMatchingCurrentUserId()
    {
        //Arrange
        var command = BudgetsTestsData.CorrectUpdateNameCommand;
        _currentUserService.UserId.Returns("DifferentUserId");
        
        //Act
        var response = await _client.PatchAsJsonAsync(EndpointPath(), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}