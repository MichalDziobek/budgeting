using System.Net;
using System.Net.Http.Json;
using Application.Abstractions;
using Application.BudgetEntries.Commands.CreateBudgetEntry;
using Application.BudgetEntries.Commands.UpdateBudgetEntry;
using Application.BudgetEntries.DataModel;
using AutoFixture;
using Domain.Entities;
using FluentAssertions;
using Mapster;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using WebApi.Tests.Integration.Common;
using WebApi.Tests.Integration.Common.Abstractions;
using WebApi.Tests.Integration.Users;
using Xunit;

namespace WebApi.Tests.Integration.BudgetEntries.Command.UpdateBudgetEntry;

[Collection(nameof(SharedTestCollection))]
public class UpdateBudgetEntryTests : IAsyncLifetime
{
    private string EndpointPath(int? budgetId = null, int? budgetEntryId = null) =>
        $"budgets/{budgetId ?? OwnedBudgetId}/budgetEntries/{budgetEntryId ?? SharedBudgetId}";

    private readonly HttpClient _client;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITestDatabase _testDatabase;
    
    private List<User> _initialUsers = new();
    private List<Budget> _initialBudgets = new();
    private List<Category> _existingCategories = new();
    private List<BudgetEntry> _existingBudgetEntries = new();

    public UpdateBudgetEntryTests(CustomWebApplicationFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
        _currentUserService = apiFactory.CurrentUserService;
        _testDatabase = apiFactory.GetTestDatabase();
        
        _currentUserService.UserId.Returns(UserTestsData.DefaultUserId);
        var fixture = new Fixture();

        PrepareData(fixture);

        _currentUserService.UserId.Returns(OwnerId);

    }

    public async Task InitializeAsync()
    {
        await _testDatabase.AddRangeAsync<User, string>(_initialUsers);
        await _testDatabase.AddRangeAsync<Budget, int>(_initialBudgets);
        await _testDatabase.AddRangeAsync<Category, int>(_existingCategories);
        await _testDatabase.AddRangeAsync<BudgetEntry, int>(_existingBudgetEntries);
    }

    public async Task DisposeAsync() => await _testDatabase.ResetAsync();

    [Fact]
    public async Task Update_ShouldReturnOk_WhenCorrectRequestForOwner()
    {
        //Arrange
        var command = BudgetEntriesTestsData.CorrectUpdateCommand(OwnedBudgetEntryId, OtherCategoryId);
        
        //Act
        var response = await _client.PutAsJsonAsync(EndpointPath(OwnedBudgetId, OwnedBudgetEntryId), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Update_ShouldReturnOk_WhenCorrectRequestForShared()
    {
        //Arrange
        var command = BudgetEntriesTestsData.CorrectUpdateCommand(SharedBudgetEntryId, OtherCategoryId);
        
        //Act
        var response = await _client.PutAsJsonAsync(EndpointPath(SharedBudgetId, SharedBudgetEntryId), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Update_ShouldReturnId_WhenCorrectRequest()
    {
        //Arrange
        var command = BudgetEntriesTestsData.CorrectUpdateCommand(OwnedBudgetEntryId, OtherCategoryId);
        
        //Act
        var response = await _client.PutAsJsonAsync(EndpointPath(OwnedBudgetId, OwnedBudgetEntryId), command);
        var result = await response.Content.ReadFromJsonAsync<UpdateBudgetEntryResponse>();
        
        //Assert
        result.Should().NotBeNull();
        result!.BudgetEntry.Id.Should().NotBe(default);
    }
    
    [Fact]
    public async Task Update_ShouldReturnCorrectResponse_WhenCorrectRequest()
    {
        //Arrange
        var command = BudgetEntriesTestsData.CorrectUpdateCommand(OwnedBudgetEntryId, OtherCategoryId);
        var expected = new CreateBudgetEntryResponse()
        {
            BudgetEntry = BudgetEntriesTestsData.DefaultEntity(OwnedBudgetEntryId, OtherCategoryId).Adapt<BudgetEntryDto>()
        };
        command.Adapt(expected.BudgetEntry);

        //Act
        var response = await _client.PutAsJsonAsync(EndpointPath(OwnedBudgetId, OwnedBudgetEntryId), command);
        var result = await response.Content.ReadFromJsonAsync<UpdateBudgetEntryResponse>();
        
        //Assert
        result.Should().NotBeNull();
        result!.Should().BeEquivalentTo(expected, x=> x.Excluding(y =>  y.BudgetEntry.Id));
    }
    
    [Fact]
    public async Task Update_ShouldUpdateBudgetTotalValue_WhenCorrectRequest()
    {
        //Arrange
        var command = BudgetEntriesTestsData.CorrectUpdateCommand(OwnedBudgetEntryId, SelectedCategoryId);
        var budget = await _testDatabase.FindAsync<Budget, int>(OwnedBudgetId);
        var budgetEntry = await _testDatabase.FindAsync<BudgetEntry, int>(OwnedBudgetEntryId);

        var expected = budget?.TotalValue - budgetEntry?.Value + command.Value;
        
        //Act
        _ = await _client.PutAsJsonAsync(EndpointPath(OwnedBudgetId, OwnedBudgetEntryId), command);
        var entity = await _testDatabase.FindAsync<Budget, int>(OwnedBudgetId);
        
        //Assert
        entity.Should().NotBeNull();
        entity!.TotalValue.Should().Be(expected);
    }
    
    [Fact]
    public async Task Update_ShouldUpdateDb_WhenCorrectRequest()
    {
        //Arrange
        var command = BudgetEntriesTestsData.CorrectUpdateCommand(OwnedBudgetEntryId, OtherCategoryId);
        var expected = BudgetEntriesTestsData.DefaultEntity(OwnedBudgetId, OtherCategoryId);
        command.Adapt(expected);
        expected.Id = command.BudgetEntryId;
        
        //Act
        var response = await _client.PutAsJsonAsync(EndpointPath(OwnedBudgetId, OwnedBudgetEntryId), command);
        var result = await response.Content.ReadFromJsonAsync<UpdateBudgetEntryResponse>();
        var entity = await _testDatabase.FindAsync<BudgetEntry, int>(result?.BudgetEntry.Id ?? default);
        
        //Assert
        entity.Should().BeEquivalentTo(expected, x => x
            .Excluding(y => y.Budget)
            .Excluding(y => y.Category)
            .Excluding(y => y.DomainEvents));
    }
    
    [Theory]
    [InlineData("")]
    public async Task Update_ShouldReturnBadRequest_IncorrectRequestData(string name)
    {
        //Arrange
        var command = new CreateBudgetEntryCommand()
        {
            Name = name,
            BudgetId = OwnedBudgetEntryId
        };
        
        //Act
        var response = await _client.PutAsJsonAsync(EndpointPath(OwnedBudgetId, OwnedBudgetEntryId), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Update_ShouldReturnUnauthorized_WhenUserIdIsEmpty()
    {
        //Arrange
        var command = BudgetEntriesTestsData.CorrectUpdateCommand(OwnedBudgetEntryId, SelectedCategoryId);
        _currentUserService.UserId.ReturnsNull();
        
        //Act
        var response = await _client.PutAsJsonAsync(EndpointPath(OwnedBudgetId, OwnedBudgetEntryId), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenBudgetDoesNotExists()
    {
        //Arrange
        const int notExistingBudgetId = -1;
        var command = BudgetEntriesTestsData.CorrectUpdateCommand(notExistingBudgetId, SelectedCategoryId);
        
        //Act
        var response = await _client.PutAsJsonAsync(EndpointPath(notExistingBudgetId), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenCategoryDoesNotExists()
    {
        //Arrange
        const int notExistingCategoryId = -1;
        var command = BudgetEntriesTestsData.CorrectUpdateCommand(OwnedBudgetEntryId, notExistingCategoryId);
        
        //Act
        var response = await _client.PutAsJsonAsync(EndpointPath(OwnedBudgetId, OwnedBudgetEntryId), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Update_ShouldReturnForbidden_WhenUserDoesNotHaveAccess()
    {
        //Arrange
        _currentUserService.UserId.Returns(OtherUserId);
        var command = BudgetEntriesTestsData.CorrectUpdateCommand(OwnedBudgetEntryId, SelectedCategoryId);
        
        //Act
        var response = await _client.PutAsJsonAsync(EndpointPath(OwnedBudgetId, OwnedBudgetEntryId), command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    private string OwnerId => _initialUsers[0].Id;
    private string OtherUserId => _initialUsers[1].Id;
    private int OwnedBudgetId => _initialBudgets[0].Id;
    private int SharedBudgetId => _initialBudgets[1].Id;
    private int OwnedBudgetEntryId => _existingBudgetEntries[0].Id;
    private int SharedBudgetEntryId => _existingBudgetEntries[1].Id;
    private int SelectedCategoryId => _existingCategories[0].Id;
    private int OtherCategoryId => _existingCategories[0].Id;
    
    private void PrepareData(Fixture fixture)
    {
        _initialUsers = new List<User>()
        {
            new() { Id = fixture.Create<string>(), FullName = "John Doe", Email = "john.doe@example.com" },
            new() { Id = fixture.Create<string>(), FullName = "Jane Doe", Email = "jane.doe@example.com" },
        };
        _initialBudgets = new List<Budget>()
        {
            new() { Id = fixture.Create<int>(), Name = "Budget 1", OwnerId = OwnerId, TotalValue = 1000 },
            new() { Id = fixture.Create<int>(), Name = "Budget 2", OwnerId = OtherUserId, TotalValue = 1000 },
        };

        _existingCategories = new List<Category>()
        {
            new() { Id = fixture.Create<int>(), Name = "Category 1" },
            new() { Id = fixture.Create<int>(), Name = "Category 2" },
        };

        _initialBudgets[1].SharedBudgets = new List<SharedBudget>()
            { new() { BudgetId = _initialBudgets[1].Id, UserId = OwnerId } };

        _existingBudgetEntries = new List<BudgetEntry>()
        {
            new()
            {
                Id = fixture.Create<int>(), Name = fixture.Create<string>(), Value = 200,
                BudgetId = _initialBudgets[0].Id, CategoryId = _existingCategories[0].Id,
            },
            new()
            {
                Id = fixture.Create<int>(), Name = fixture.Create<string>(), Value = 200,
                BudgetId = _initialBudgets[1].Id, CategoryId = _existingCategories[0].Id,
            }
        };
    }
}