using System.Net;
using System.Net.Http.Json;
using Application.Abstractions;
using Application.BudgetEntries.Commands.CreateBudgetEntry;
using Application.BudgetEntries.DataModel;
using Application.Budgets.Commands.CreateBudget;
using Application.Budgets.DataModels;
using AutoFixture;
using Domain.Entities;
using FluentAssertions;
using Mapster;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using WebApi.Tests.Integration.Budgets;
using WebApi.Tests.Integration.Common;
using WebApi.Tests.Integration.Common.Abstractions;
using WebApi.Tests.Integration.Users;
using Xunit;

namespace WebApi.Tests.Integration.BudgetEntries.Command.CreateBudgetEntry;

[Collection(nameof(SharedTestCollection))]
public class CreateBudgetEntryTests : IAsyncLifetime
{
    private string EndpointPath(int? budgetId = null) => $"budgets/{budgetId ?? _ownedBudgetId}/budgetEntries/";

    private readonly HttpClient _client;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITestDatabase _testDatabase;
    private readonly int _ownedBudgetId;
    private readonly int _sharedBudgetId;
    
    private readonly List<User> _initialUsers;
    private readonly List<Budget> _initialBudgets;
    private readonly Category _existingCategory;

    public CreateBudgetEntryTests(CustomWebApplicationFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
        _currentUserService = apiFactory.CurrentUserService;
        _testDatabase = apiFactory.GetTestDatabase();
        
        _currentUserService.UserId.Returns(UserTestsData.DefaultUserId);
        var fixture = new Fixture();

        _initialUsers = new List<User>()
        {
            new() { Id = fixture.Create<string>(), FullName = "John Doe", Email = "john.doe@example.com" },
            new() { Id = fixture.Create<string>(), FullName = "Jane Doe", Email = "jane.doe@example.com" },
        };
        _initialBudgets = new List<Budget>()
        {
            new() { Id = fixture.Create<int>(), Name = "Budget 1", OwnerId = OwnerId },
            new() { Id = fixture.Create<int>(), Name = "Budget 2", OwnerId = OtherUserId },
        };
        _ownedBudgetId = _initialBudgets[0].Id;
        _sharedBudgetId = _initialBudgets[1].Id;
        
        _existingCategory = new() { Id = fixture.Create<int>(), Name = "Category 1" };
        
        _initialBudgets[1].SharedBudgets = new List<SharedBudget>()
            { new() { BudgetId = _initialBudgets[1].Id, UserId = OwnerId } };

        _currentUserService.UserId.Returns(OwnerId);

    }

    public async Task InitializeAsync()
    {
        await _testDatabase.AddRangeAsync<User, string>(_initialUsers);
        await _testDatabase.AddRangeAsync<Budget, int>(_initialBudgets);
        await _testDatabase.AddAsync<Category, int>(_existingCategory);
    }

    public async Task DisposeAsync() => await _testDatabase.ResetAsync();

    [Fact]
    public async Task Create_ShouldReturnOk_OnCorrectRequestForOwner()
    {
        //Arrange
        var command = BudgetEntriesTestsData.CorrectCreateCommand(_ownedBudgetId, _existingCategory.Id);
        
        //Act
        var response = await _client.PostAsJsonAsync(EndpointPath(_ownedBudgetId), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Create_ShouldReturnOk_OnCorrectRequestForShared()
    {
        //Arrange
        var command = BudgetEntriesTestsData.CorrectCreateCommand(_sharedBudgetId, _existingCategory.Id);
        
        //Act
        var response = await _client.PostAsJsonAsync(EndpointPath(_sharedBudgetId), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Create_ShouldReturnId_OnCorrectRequest()
    {
        //Arrange
        var command = BudgetEntriesTestsData.CorrectCreateCommand(_ownedBudgetId, _existingCategory.Id);
        
        //Act
        var response = await _client.PostAsJsonAsync(EndpointPath(_ownedBudgetId), command);
        var result = await response.Content.ReadFromJsonAsync<CreateBudgetEntryResponse>();
        
        //Assert
        result.Should().NotBeNull();
        result!.BudgetEntry.Id.Should().NotBe(default);
    }
    
    [Fact]
    public async Task Create_ShouldReturnCorrectResponse_OnCorrectRequest()
    {
        //Arrange
        var command = BudgetEntriesTestsData.CorrectCreateCommand(_ownedBudgetId, _existingCategory.Id);
        var expected = new CreateBudgetEntryResponse()
        {
            BudgetEntry = BudgetEntriesTestsData.DefaultEntity(_ownedBudgetId, _existingCategory.Id).Adapt<BudgetEntryDto>()
        };

        //Act
        var response = await _client.PostAsJsonAsync(EndpointPath(_ownedBudgetId), command);
        var result = await response.Content.ReadFromJsonAsync<CreateBudgetEntryResponse>();
        
        //Assert
        result.Should().NotBeNull();
        result!.Should().BeEquivalentTo(expected, x=> x.Excluding(y =>  y.BudgetEntry.Id));
    }
    
    [Fact]
    public async Task Create_ShouldAddToDb_OnCorrectRequest()
    {
        //Arrange
        var command = BudgetEntriesTestsData.CorrectCreateCommand(_ownedBudgetId, _existingCategory.Id);
        var expected = BudgetEntriesTestsData.DefaultEntity(_ownedBudgetId, _existingCategory.Id);
        
        //Act
        var response = await _client.PostAsJsonAsync(EndpointPath(_ownedBudgetId), command);
        var result = await response.Content.ReadFromJsonAsync<CreateBudgetEntryResponse>();
        var entity = await _testDatabase.FindAsync<BudgetEntry, int>(result?.BudgetEntry?.Id ?? default);
        
        //Assert
        entity.Should().BeEquivalentTo(expected, x => x.Excluding(y => y.Id)
            .Excluding(y => y.Budget)
            .Excluding(y => y.Category));
    }
    
    [Theory]
    [InlineData("")]
    public async Task Create_ShouldReturnBadRequest_IncorrectRequestData(string name)
    {
        //Arrange
        var command = new CreateBudgetEntryCommand()
        {
            Name = name,
            BudgetId = _ownedBudgetId
        };
        
        //Act
        var response = await _client.PostAsJsonAsync(EndpointPath(_ownedBudgetId), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Create_ShouldReturnUnauthorized_WhenUserIdIsEmpty()
    {
        //Arrange
        var command = BudgetEntriesTestsData.CorrectCreateCommand(_ownedBudgetId, _existingCategory.Id);
        _currentUserService.UserId.ReturnsNull();
        
        //Act
        var response = await _client.PostAsJsonAsync(EndpointPath(_ownedBudgetId), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenBudgetDoesNotExists()
    {
        //Arrange
        var notExistingBudgetId = -1;
        var command = BudgetEntriesTestsData.CorrectCreateCommand(notExistingBudgetId, _existingCategory.Id);
        
        //Act
        var response = await _client.PostAsJsonAsync(EndpointPath(notExistingBudgetId), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenCategoryDoesNotExists()
    {
        //Arrange
        var notExistingCategoryId = -1;
        var command = BudgetEntriesTestsData.CorrectCreateCommand(_ownedBudgetId, notExistingCategoryId);
        
        //Act
        var response = await _client.PostAsJsonAsync(EndpointPath(_ownedBudgetId), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Create_ShouldReturnForbidden_WhenUserDoesNotHaveAccess()
    {
        //Arrange
        _currentUserService.UserId.Returns(OtherUserId);
        var command = BudgetEntriesTestsData.CorrectCreateCommand(_ownedBudgetId, _existingCategory.Id);
        
        //Act
        var response = await _client.PostAsJsonAsync(EndpointPath(_ownedBudgetId), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    private string OwnerId => _initialUsers[0].Id;
    private string OtherUserId => _initialUsers[1].Id;
}