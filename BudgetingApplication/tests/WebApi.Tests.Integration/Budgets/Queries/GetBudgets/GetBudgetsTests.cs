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
    private readonly List<Budget> _initialBudgets;
    private readonly List<User> _initialUsers;
    private readonly ICurrentUserService _currentUserService;

    public GetBudgetsTests(CustomWebApplicationFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
        _testDatabase = apiFactory.GetTestDatabase();
        _currentUserService = apiFactory.CurrentUserService;

        var fixture = new Fixture();
        
        _initialUsers = new List<User>()
        {
            new() { Id = fixture.Create<string>(), FullName = "John Doe", Email = "john.doe@example.com" },
            new() { Id = fixture.Create<string>(), FullName = "Jane Doe", Email = "jane.doe@example.com" },
        };
        _initialBudgets = new List<Budget>()
        {
            new() { Id = fixture.Create<int>(), Name = "Budget 1", OwnerId = _initialUsers[0].Id },
            new() { Id = fixture.Create<int>(), Name = "Budget 2", OwnerId = _initialUsers[0].Id },
            new() { Id = fixture.Create<int>(), Name = "Budget 3", OwnerId = _initialUsers[1].Id },
            new() { Id = fixture.Create<int>(), Name = "Budget 4", OwnerId = _initialUsers[1].Id, UsersWithSharedAccess = new List<User>(){ _initialUsers[0] }},
            new() { Id = fixture.Create<int>(), Name = "Budget 5", OwnerId = _initialUsers[1].Id, UsersWithSharedAccess = new List<User>(){ _initialUsers[0] }},
        };
    }

    public async Task InitializeAsync()
    {
        await _testDatabase.AddRangeAsync<User, string>(_initialUsers);
        await _testDatabase.AddRangeAsync<Budget, int>(_initialBudgets);
    }

    public async Task DisposeAsync() => await _testDatabase.ResetAsync();
    
    [Fact]
    public async Task Get_ShouldReturnOk_OnEmptyQuery()
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
    public async Task Get_ShouldReturnBudgetsForCorrectUser_OnEmptyQuery(int userIndex)
    {
        //Arrange
        _currentUserService.UserId.Returns(_initialUsers[userIndex].Id);
        var expectedResult = new GetBudgetsResponse
        {
            OwnedBudgets = _initialBudgets.Where(x => x.OwnerId == _currentUserService.UserId).Adapt<IEnumerable<BudgetDto>>(),
            SharedBudgets = _initialBudgets.Where(x => x.UsersWithSharedAccess.Any(y => y.Id == _currentUserService.UserId))
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
}