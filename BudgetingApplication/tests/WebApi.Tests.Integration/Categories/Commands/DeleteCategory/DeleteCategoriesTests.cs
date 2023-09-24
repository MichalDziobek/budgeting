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
    private Category _existingCategory = new Category();
    private readonly ITestPermissionsProvider _testPermissionsProvider;

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

    private void PrepareData()
    {
        var fixture = new Fixture();
        _existingCategory = new Category() { Id = fixture.Create<int>(), Name = "Category Name" };
    }

    public async Task InitializeAsync() => await _testDatabase.AddAsync<Category, int>(_existingCategory);

    public async Task DisposeAsync() => await _testDatabase.ResetAsync();

    [Fact]
    public async Task Create_ShouldReturnOk_OnCorrectRequest()
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
    public async Task Create_ShouldDeleteFromDb_OnCorrectRequest()
    {
        //Arrange
        
        //Act
        _ = await _client.DeleteAsync(EndpointPath(_existingCategory.Id));
        var entity = await _testDatabase.FindAsync<Category, int>(_existingCategory.Id);

        //Assert
        entity.Should().BeNull();
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
}