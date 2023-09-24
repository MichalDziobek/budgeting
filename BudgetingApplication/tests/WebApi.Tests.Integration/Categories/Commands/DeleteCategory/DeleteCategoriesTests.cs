using System.Net;
using AutoFixture;
using Domain.Entities;
using FluentAssertions;
using NSubstitute;
using WebApi.Authorization;
using WebApi.Tests.Integration.Common;
using WebApi.Tests.Integration.Common.Abstractions;
using WebApi.Tests.Integration.Users;
using Xunit;

namespace WebApi.Tests.Integration.Categories.Commands.DeleteCategory;

[Collection(nameof(SharedTestCollection))]
public class DeleteCategoriesTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly ITestDatabase _testDatabase;
    private readonly ITestPermissionsProvider _testPermissionsProvider;
    private Category _existingCategory = new();
    private List<User> _initialUsers = new();
    private List<Budget> _initialBudgets = new();
    private List<BudgetEntry> _existingBudgetEntries = new();

    private string EndpointPath(int categoryId) => $"categories/{categoryId}";

    public DeleteCategoriesTests(CustomWebApplicationFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
        _testDatabase = apiFactory.GetTestDatabase();
        _testPermissionsProvider = apiFactory.TestPermissionsProvider;
        _testPermissionsProvider.GetPermissionValues()
            .Returns(new[] { AuthorizationPolicies.DeleteCategoryPolicy.PermissionName });

        PrepareData();

        apiFactory.CurrentUserService.UserId.Returns(UserTestsData.DefaultUserId);
        
    }

    public async Task InitializeAsync()
    {
        await _testDatabase.AddRangeAsync<User, string>(_initialUsers);
        await _testDatabase.AddRangeAsync<Budget, int>(_initialBudgets);
        await _testDatabase.AddAsync<Category, int>(_existingCategory);
        await _testDatabase.AddRangeAsync<BudgetEntry, int>(_existingBudgetEntries);
    }

    public async Task DisposeAsync() => await _testDatabase.ResetAsync();

    [Fact]
    public async Task Create_ShouldReturnOk_WhenCorrectRequest()
    {
        //Arrange
        
        //Act
        var response = await _client.DeleteAsync(EndpointPath(_existingCategory.Id));
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_ShouldForbidden_WhenDoesNotHavePermission()
    {
        //Arrange
        _testPermissionsProvider.GetPermissionValues().Returns(Enumerable.Empty<string>());
        
        //Act
        var response = await _client.DeleteAsync(EndpointPath(_existingCategory.Id));
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task Create_ShouldDeleteCategoryFromDb_WhenCorrectRequest()
    {
        //Arrange
        
        //Act
        _ = await _client.DeleteAsync(EndpointPath(_existingCategory.Id));
        var entity = await _testDatabase.FindAsync<Category, int>(_existingCategory.Id);

        //Assert
        entity.Should().BeNull();
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task Create_ShouldDeleteBudgetEntriesDb_WhenCorrectRequest(int budgetEntryIndex)
    {
        //Arrange
        var budgetEntryId = _existingBudgetEntries[budgetEntryIndex].Id;
        
        //Act
        var entityPreDelete = await _testDatabase.FindAsync<BudgetEntry, int>(budgetEntryId);
        _ = await _client.DeleteAsync(EndpointPath(_existingCategory.Id));
        var entityPostDelete = await _testDatabase.FindAsync<BudgetEntry, int>(budgetEntryId);

        //Assert
        entityPreDelete.Should().NotBeNull();
        entityPostDelete.Should().BeNull();
    }

    
    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenCategoryDoesNotExists()
    {
        //Arrange
        
        //Act
        var response = await _client.DeleteAsync(EndpointPath(_existingCategory.Id + 1));
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private void PrepareData()
    {
        var fixture = new Fixture();
        _existingCategory = new Category() { Id = fixture.Create<int>(), Name = "Category Name" };
        _initialUsers = new List<User>()
        {
            new() { Id = fixture.Create<string>(), FullName = "John Doe", Email = "john.doe@example.com" },
        };
        _initialBudgets = new List<Budget>()
        {
            new() { Id = fixture.Create<int>(), Name = "Budget 1", OwnerId = _initialUsers[0].Id, TotalValue = 1000 },
        };

        _existingBudgetEntries = new List<BudgetEntry>()
        {
            new()
            {
                Id = fixture.Create<int>(), Name = fixture.Create<string>(), Value = 200,
                BudgetId = _initialBudgets[0].Id, CategoryId = _existingCategory.Id,
            },
            new()
            {
                Id = fixture.Create<int>(), Name = fixture.Create<string>(), Value = 200,
                BudgetId = _initialBudgets[0].Id, CategoryId = _existingCategory.Id,
            }
        };
    }
}