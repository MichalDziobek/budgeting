using System.Net;
using System.Net.Http.Json;
using Application.Abstractions;
using Application.Budgets.DataModels;
using Application.Budgets.Queries.GetBudgets;
using AutoFixture;
using Domain.Entities;
using FluentAssertions;
using Mapster;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using WebApi.Tests.Integration.Common;
using WebApi.Tests.Integration.Common.Abstractions;
using Xunit;

namespace WebApi.Tests.Integration.Budgets.Queries.GetBudgets;

[Collection(nameof(SharedTestCollection))]
public class GetBudgetsTests : IAsyncLifetime
{
    private const string PathPrefix = "budgets";
    private readonly HttpClient _client;
    private readonly ITestDatabase _testDatabase;
    private readonly ICurrentUserService _currentUserService;
    private List<Budget> _initialBudgets = new();
    private List<User> _initialUsers = new();

    public GetBudgetsTests(CustomWebApplicationFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
        _testDatabase = apiFactory.GetTestDatabase();
        _currentUserService = apiFactory.CurrentUserService;

        PrepareData();
    }


    public async Task InitializeAsync()
    {
        await _testDatabase.AddRangeAsync<User, string>(_initialUsers);
        await _testDatabase.AddRangeAsync<Budget, int>(_initialBudgets);
    }

    public async Task DisposeAsync() => await _testDatabase.ResetAsync();
    
    [Fact]
    public async Task Get_ShouldReturnOk_WhenEmptyQuery()
    {
        //Arrange
        
        //Act
        var response = await _client.GetAsync(PathPrefix);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task Get_ShouldReturnBudgetsForCorrectUser_WhenEmptyQuery(int userIndex)
    {
        //Arrange
        _currentUserService.UserId.Returns(_initialUsers[userIndex].Id);
        var expectedResult = new GetBudgetsResponse
        {
            OwnedBudgets = _initialBudgets.Where(x => x.OwnerId == _currentUserService.UserId).Adapt<IEnumerable<BudgetDto>>(),
            SharedBudgets = _initialBudgets.Where(x => x.SharedBudgets.Any(y => y.UserId == _currentUserService.UserId))
                .Adapt<IEnumerable<BudgetDto>>(),
        };

        //Act
        var response = await _client.GetAsync(PathPrefix);
        var result = await response.Content.ReadFromJsonAsync<GetBudgetsResponse>();
        
        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task Create_ShouldReturnUnauthorized_WhenUserIdIsEmpty()
    {
        //Arrange
        _currentUserService.UserId.ReturnsNull();
        
        //Act
        var response = await _client.GetAsync(PathPrefix);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private string OwnerId => _initialUsers[0].Id;
    private string SharedToUserId => _initialUsers[1].Id;
    
    
    private void PrepareData()
    {
        var fixture = new Fixture();

        _initialUsers = new List<User>()
        {
            new() { Id = fixture.Create<string>(), FullName = "John Doe", Email = "john.doe@example.com" },
            new() { Id = fixture.Create<string>(), FullName = "Jane Doe", Email = "jane.doe@example.com" },
        };
        _initialBudgets = new List<Budget>()
        {
            new() { Id = fixture.Create<int>(), Name = "Budget 1", OwnerId = OwnerId },
            new() { Id = fixture.Create<int>(), Name = "Budget 2", OwnerId = OwnerId },
            new() { Id = fixture.Create<int>(), Name = "Budget 3", OwnerId = SharedToUserId },
            new() { Id = fixture.Create<int>(), Name = "Budget 4", OwnerId = SharedToUserId },
            new() { Id = fixture.Create<int>(), Name = "Budget 5", OwnerId = SharedToUserId },
        };
        _initialBudgets[3].SharedBudgets = new List<SharedBudget>()
            { new() { BudgetId = _initialBudgets[3].Id, UserId = OwnerId } };
        _initialBudgets[4].SharedBudgets = new List<SharedBudget>()
            { new() { BudgetId = _initialBudgets[4].Id, UserId = OwnerId } };
    }
}