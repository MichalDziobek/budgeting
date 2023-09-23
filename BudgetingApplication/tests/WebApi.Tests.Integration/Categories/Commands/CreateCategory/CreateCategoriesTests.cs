using System.Net;
using System.Net.Http.Json;
using Application.Abstractions;
using Application.Budgets.DataModels;
using Application.Categories.Commands.CreateCategoryCommand;
using Application.Categories.DataModel;
using Domain.Entities;
using FluentAssertions;
using Mapster;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using WebApi.Tests.Integration.Common;
using WebApi.Tests.Integration.Common.Abstractions;
using WebApi.Tests.Integration.Users;
using Xunit;

namespace WebApi.Tests.Integration.Categories.Commands.CreateCategory;

[Collection(nameof(SharedTestCollection))]
public class CreateCategoriesTests : IAsyncLifetime
{
    private const string PathPrefix = "categories";

    private readonly HttpClient _client;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITestDatabase _testDatabase;

    public CreateCategoriesTests(CustomWebApplicationFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
        _currentUserService = apiFactory.CurrentUserService;
        _testDatabase = apiFactory.GetTestDatabase();
        
        _currentUserService.UserId.Returns(UserTestsData.DefaultUserId);
        
    }

    public async Task InitializeAsync() => await _testDatabase.AddAsync<User, string>(UserTestsData.DefaultUser);

    public async Task DisposeAsync() => await _testDatabase.ResetAsync();

    [Fact]
    public async Task Create_ShouldReturnOk_OnCorrectRequest()
    {
        //Arrange
        var command = CategoriesTestsData.CorrectCreateCommand;
        
        //Act
        var response = await _client.PostAsJsonAsync(PathPrefix, command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Create_ShouldReturnId_OnCorrectRequest()
    {
        //Arrange
        var command = CategoriesTestsData.CorrectCreateCommand;
        
        //Act
        var response = await _client.PostAsJsonAsync(PathPrefix, command);
        var result = await response.Content.ReadFromJsonAsync<CreateCategoryResponse>();
        
        //Assert
        result.Should().NotBeNull();
        result!.Category.Id.Should().NotBe(default);
    }
    
    [Fact]
    public async Task Create_ShouldReturnCorrectResponse_OnCorrectRequest()
    {
        //Arrange
        var command = CategoriesTestsData.CorrectCreateCommand;
        var expected = new CreateCategoryResponse
        {
            Category = CategoriesTestsData.DefaultEntity.Adapt<CategoryDto>()
        };

        //Act
        var response = await _client.PostAsJsonAsync(PathPrefix, command);
        var result = await response.Content.ReadFromJsonAsync<CreateCategoryResponse>();
        
        //Assert
        result.Should().NotBeNull();
        result!.Should().BeEquivalentTo(expected, x=> x.Excluding(y =>  y.Category.Id));
    }
    
    [Fact]
    public async Task Create_ShouldAddToDb_OnCorrectRequest()
    {
        //Arrange
        var command = CategoriesTestsData.CorrectCreateCommand;
        var expected = CategoriesTestsData.DefaultEntity;
        
        //Act
        var response = await _client.PostAsJsonAsync(PathPrefix, command);
        var result = await response.Content.ReadFromJsonAsync<CreateCategoryResponse>();
        var entity = await _testDatabase.FindAsync<Category, int>(result?.Category?.Id ?? default);

        
        //Assert
        entity.Should().BeEquivalentTo(expected, x => x.Excluding(y => y.Id));
    }
    
    [Theory]
    [InlineData("")]
    public async Task Create_ShouldReturnBadRequest_IncorrectRequestData(string name)
    {
        //Arrange
        var command = new CreateCategoryCommand()
        {
            Name = name,
        };
        
        //Act
        var response = await _client.PostAsJsonAsync(PathPrefix, command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenAlreadyExists()
    {
        //Arrange
        var command = CategoriesTestsData.CorrectCreateCommand;
        await _testDatabase.AddAsync<Category, int>(CategoriesTestsData.DefaultEntity);
        
        //Act
        var response = await _client.PostAsJsonAsync(PathPrefix, command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}