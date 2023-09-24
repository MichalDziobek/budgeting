using System.Net;
using System.Net.Http.Json;
using Application.Abstractions;
using Application.BudgetEntries.DataModel;
using Application.BudgetEntries.Queries.GetBudgetEntries;
using Application.Categories.DataModel;
using Application.Categories.Queries.GetCategories;
using Application.DataModels.Common;
using AutoFixture;
using Domain.Entities;
using FluentAssertions;
using Mapster;
using Microsoft.AspNetCore.WebUtilities;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using WebApi.Tests.Integration.Common;
using WebApi.Tests.Integration.Common.Abstractions;
using Xunit;

namespace WebApi.Tests.Integration.BudgetEntries.Queries;

[Collection(nameof(SharedTestCollection))]
public class GetBudgetEntriesTests : IAsyncLifetime
{
    private string EndpointPath(int? id = null) => $"budgets/{id ?? _ownedBudgetId}/budgetEntries";
    private readonly HttpClient _client;
    private readonly ITestDatabase _testDatabase;
    private readonly List<Category> _initialCategories;
    private readonly List<User> _initialUsers;
    private readonly List<Budget> _initialBudgets;
    private readonly List<BudgetEntry> _budgetEntries;
    private readonly ICurrentUserService _currentUserService;
    
    private readonly int _ownedBudgetId;
    private readonly int _sharedBudgetId;

    public GetBudgetEntriesTests(CustomWebApplicationFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
        _testDatabase = apiFactory.GetTestDatabase();

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
        _initialBudgets[1].SharedBudgets = new List<SharedBudget>(){ new(){ BudgetId = _sharedBudgetId, UserId = OwnerId}};
        
        _initialCategories = new List<Category>()
        {
            new() { Id = fixture.Create<int>(), Name = "Expense - Other"},
            new() { Id = fixture.Create<int>(), Name = "Income - Other"},
        };
        
        _budgetEntries = new List<BudgetEntry>()
        {
            new() { Id = fixture.Create<int>(), Name = fixture.Create<string>(), BudgetId = _ownedBudgetId, CategoryId = _initialCategories[0].Id, Value = -100},
            new() { Id = fixture.Create<int>(), Name = fixture.Create<string>(), BudgetId = _ownedBudgetId, CategoryId = _initialCategories[0].Id, Value = 100},
            new() { Id = fixture.Create<int>(), Name = fixture.Create<string>(), BudgetId = _ownedBudgetId, CategoryId = _initialCategories[1].Id, Value = -100},
            new() { Id = fixture.Create<int>(), Name = fixture.Create<string>(), BudgetId = _ownedBudgetId, CategoryId = _initialCategories[1].Id, Value = 100},
            new() { Id = fixture.Create<int>(), Name = fixture.Create<string>(), BudgetId = _sharedBudgetId, CategoryId = _initialCategories[0].Id, Value = -100},
            new() { Id = fixture.Create<int>(), Name = fixture.Create<string>(), BudgetId = _sharedBudgetId, CategoryId = _initialCategories[0].Id, Value = 100},
            new() { Id = fixture.Create<int>(), Name = fixture.Create<string>(), BudgetId = _sharedBudgetId, CategoryId = _initialCategories[1].Id, Value = -100},
            new() { Id = fixture.Create<int>(), Name = fixture.Create<string>(), BudgetId = _sharedBudgetId, CategoryId = _initialCategories[1].Id, Value = 100},
        };

        apiFactory.CurrentUserService.UserId.Returns(OwnerId);
        _currentUserService = apiFactory.CurrentUserService;
    }

    public async Task InitializeAsync()
    {
        await _testDatabase.AddRangeAsync<User, string>(_initialUsers);
        await _testDatabase.AddRangeAsync<Category, int>(_initialCategories);
        await _testDatabase.AddRangeAsync<Budget, int>(_initialBudgets);
        await _testDatabase.AddRangeAsync<BudgetEntry, int>(_budgetEntries);
    }

    public async Task DisposeAsync() => await _testDatabase.ResetAsync();
    
    [Fact]
    public async Task Get_ShouldReturnOk_OnCorrectQuery_ForOwnBudget()
    {
        //Arrange
        var queryParams = new Dictionary<string, string?>()
        {
            { nameof(GetBudgetEntriesQuery.Offset), 0.ToString() },
            { nameof(GetBudgetEntriesQuery.Limit), _budgetEntries.Count.ToString() },
            { nameof(GetBudgetEntriesQuery.BudgetId), _ownedBudgetId.ToString() },
            { nameof(GetBudgetEntriesQuery.CategoryFilter), null },
            { nameof(GetBudgetEntriesQuery.BudgetEntryTypeFilter), null },
        };
        var expectedResult = new GetBudgetEntriesResponse()
        {
            BudgetEntries = PaginatedResponse<BudgetEntryDto>.AdaptFrom(_budgetEntries.Where(x => x.BudgetId == _ownedBudgetId)) 
        };
        var url = QueryHelpers.AddQueryString(EndpointPath(), queryParams);
        
        //Act
        var response = await _client.GetAsync(url);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Get_ShouldReturnOk_OnCorrectQuery_ForSharedBudget()
    {
        //Arrange
        var queryParams = new Dictionary<string, string?>()
        {
            { nameof(GetBudgetEntriesQuery.Offset), 0.ToString() },
            { nameof(GetBudgetEntriesQuery.Limit), _budgetEntries.Count.ToString() },
            { nameof(GetBudgetEntriesQuery.BudgetId), _sharedBudgetId.ToString() },
            { nameof(GetBudgetEntriesQuery.CategoryFilter), null },
            { nameof(GetBudgetEntriesQuery.BudgetEntryTypeFilter), null },
        };

        var url = QueryHelpers.AddQueryString(EndpointPath(_sharedBudgetId), queryParams);
        
        //Act
        var response = await _client.GetAsync(url);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Get_ShouldReturnExpectedEntries_OnNonFilteredQuery_ForOwnBudget()
    {
        //Arrange
        var queryParams = new Dictionary<string, string?>()
        {
            { nameof(GetBudgetEntriesQuery.Offset), 0.ToString() },
            { nameof(GetBudgetEntriesQuery.Limit), _budgetEntries.Count.ToString() },
            { nameof(GetBudgetEntriesQuery.BudgetId), _ownedBudgetId.ToString() },
            { nameof(GetBudgetEntriesQuery.CategoryFilter), null },
            { nameof(GetBudgetEntriesQuery.BudgetEntryTypeFilter), null },
        };
        var expectedResult = new GetBudgetEntriesResponse()
        {
            BudgetEntries = PaginatedResponse<BudgetEntryDto>.AdaptFrom(_budgetEntries.Where(x => x.BudgetId == _ownedBudgetId)) 
        };
        var url = QueryHelpers.AddQueryString(EndpointPath(), queryParams);
        
        //Act
        var response = await _client.GetAsync(url);
        var result = await response.Content.ReadFromJsonAsync<GetBudgetEntriesResponse>();
        
        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
    
    [Fact]
    public async Task Get_ShouldReturnExpectedEntries_OnQueryFilteredByCategory_ForOwnBudget()
    {
        //Arrange
        var queryParams = new Dictionary<string, string?>()
        {
            { nameof(GetBudgetEntriesQuery.Offset), 0.ToString() },
            { nameof(GetBudgetEntriesQuery.Limit), _budgetEntries.Count.ToString() },
            { nameof(GetBudgetEntriesQuery.BudgetId), _ownedBudgetId.ToString() },
            { nameof(GetBudgetEntriesQuery.CategoryFilter), _initialCategories[0].Id.ToString() },
            { nameof(GetBudgetEntriesQuery.BudgetEntryTypeFilter), null },
        };
        var expectedResult = new GetBudgetEntriesResponse()
        {
            BudgetEntries = PaginatedResponse<BudgetEntryDto>.AdaptFrom(_budgetEntries.Where(x =>
                x.BudgetId == _ownedBudgetId && x.CategoryId == _initialCategories[0].Id)) 
        };
        var url = QueryHelpers.AddQueryString(EndpointPath(), queryParams);
        
        //Act
        var response = await _client.GetAsync(url);
        var result = await response.Content.ReadFromJsonAsync<GetBudgetEntriesResponse>();
        
        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
    
    [Fact]
    public async Task Get_ShouldReturnExpectedEntries_OnQueryFilteredByType_ForOwnBudget()
    {
        //Arrange
        var queryParams = new Dictionary<string, string?>()
        {
            { nameof(GetBudgetEntriesQuery.Offset), 0.ToString() },
            { nameof(GetBudgetEntriesQuery.Limit), _budgetEntries.Count.ToString() },
            { nameof(GetBudgetEntriesQuery.BudgetId), _ownedBudgetId.ToString() },
            { nameof(GetBudgetEntriesQuery.CategoryFilter), null },
            { nameof(GetBudgetEntriesQuery.BudgetEntryTypeFilter), 0.ToString() },
        };
        var expectedResult = new GetBudgetEntriesResponse()
        {
            BudgetEntries = PaginatedResponse<BudgetEntryDto>.AdaptFrom(_budgetEntries.Where(x =>
                x.BudgetId == _ownedBudgetId && x.Value > 0)) 
        };
        var url = QueryHelpers.AddQueryString(EndpointPath(), queryParams);
        
        //Act
        var response = await _client.GetAsync(url);
        var result = await response.Content.ReadFromJsonAsync<GetBudgetEntriesResponse>();
        
        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
    
    [Fact]
    public async Task Get_ShouldReturnExpectedEntries_OnQueryFilteredByTypeAndCategory_ForOwnBudget()
    {
        //Arrange
        var queryParams = new Dictionary<string, string?>()
        {
            { nameof(GetBudgetEntriesQuery.Offset), 0.ToString() },
            { nameof(GetBudgetEntriesQuery.Limit), _budgetEntries.Count.ToString() },
            { nameof(GetBudgetEntriesQuery.BudgetId), _ownedBudgetId.ToString() },
            { nameof(GetBudgetEntriesQuery.CategoryFilter), _initialCategories[0].Id.ToString() },
            { nameof(GetBudgetEntriesQuery.BudgetEntryTypeFilter), 0.ToString() },
        };
        var expectedResult = new GetBudgetEntriesResponse()
        {
            BudgetEntries = PaginatedResponse<BudgetEntryDto>.AdaptFrom(_budgetEntries.Where(x =>
                x.BudgetId == _ownedBudgetId && x.Value > 0 && x.CategoryId == _initialCategories[0].Id)) 
        };
        var url = QueryHelpers.AddQueryString(EndpointPath(), queryParams);
        
        //Act
        var response = await _client.GetAsync(url);
        var result = await response.Content.ReadFromJsonAsync<GetBudgetEntriesResponse>();
        
        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
    
    [Fact]
    public async Task Get_ShouldReturnNotFound_OnNonExistingBudget()
    {
        //Arrange
        const int nonExistingBudgetId = -1;
        var queryParams = new Dictionary<string, string?>()
        {
            { nameof(GetBudgetEntriesQuery.Offset), 0.ToString() },
            { nameof(GetBudgetEntriesQuery.Limit), _budgetEntries.Count.ToString() },
            { nameof(GetBudgetEntriesQuery.BudgetId), nonExistingBudgetId.ToString() },
            { nameof(GetBudgetEntriesQuery.CategoryFilter), null },
            { nameof(GetBudgetEntriesQuery.BudgetEntryTypeFilter), null },
        };

        var url = QueryHelpers.AddQueryString(EndpointPath(nonExistingBudgetId), queryParams);
        
        //Act
        var response = await _client.GetAsync(url);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Get_ShouldReturnUnauthorized_OnNullUserId()
    {
        //Arrange
        var queryParams = new Dictionary<string, string?>()
        {
            { nameof(GetBudgetEntriesQuery.Offset), 0.ToString() },
            { nameof(GetBudgetEntriesQuery.Limit), _budgetEntries.Count.ToString() },
            { nameof(GetBudgetEntriesQuery.BudgetId), _ownedBudgetId.ToString() },
            { nameof(GetBudgetEntriesQuery.CategoryFilter), null },
            { nameof(GetBudgetEntriesQuery.BudgetEntryTypeFilter), null },
        };
        _currentUserService.UserId.ReturnsNull();

        var url = QueryHelpers.AddQueryString(EndpointPath(), queryParams);
        
        //Act
        var response = await _client.GetAsync(url);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task Get_ShouldReturnForbidden_OnBudgetWithoutAccess()
    {
        //Arrange
        var queryParams = new Dictionary<string, string?>()
        {
            { nameof(GetBudgetEntriesQuery.Offset), 0.ToString() },
            { nameof(GetBudgetEntriesQuery.Limit), _budgetEntries.Count.ToString() },
            { nameof(GetBudgetEntriesQuery.BudgetId), _ownedBudgetId.ToString() },
            { nameof(GetBudgetEntriesQuery.CategoryFilter), null },
            { nameof(GetBudgetEntriesQuery.BudgetEntryTypeFilter), null },
        };
        var url = QueryHelpers.AddQueryString(EndpointPath(), queryParams);
        _currentUserService.UserId.Returns(OtherUserId);

        //Act
        var response = await _client.GetAsync(url);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    
    private string OwnerId => _initialUsers[0].Id;
    private string OtherUserId => _initialUsers[1].Id;
}