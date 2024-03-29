using System.Net;
using System.Net.Http.Json;
using Application.Abstractions;
using Application.Budgets.Commands.CreateBudget;
using Application.Budgets.DataModels;
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

namespace WebApi.Tests.Integration.Budgets.Commands.CreateBudget;

[Collection(nameof(SharedTestCollection))]
public class CreateBudgetsTests : IAsyncLifetime
{
    private const string PathPrefix = "budgets";

    private readonly HttpClient _client;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITestDatabase _testDatabase;
    private List<User> _initialUsers = new();

    public CreateBudgetsTests(CustomWebApplicationFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
        _currentUserService = apiFactory.CurrentUserService;
        _testDatabase = apiFactory.GetTestDatabase();
        
        _currentUserService.UserId.Returns(UserTestsData.DefaultUserId);
        
    }

    public async Task InitializeAsync() => await _testDatabase.AddAsync<User, string>(UserTestsData.DefaultUser);

    public async Task DisposeAsync() => await _testDatabase.ResetAsync();

    [Fact]
    public async Task Create_ShouldReturnOk_WhenCorrectRequest()
    {
        //Arrange
        var command = BudgetsTestsData.CorrectCreateCommand;
        
        //Act
        var response = await _client.PostAsJsonAsync(PathPrefix, command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Create_ShouldReturnId_WhenCorrectRequest()
    {
        //Arrange
        var command = BudgetsTestsData.CorrectCreateCommand;
        
        //Act
        var response = await _client.PostAsJsonAsync(PathPrefix, command);
        var result = await response.Content.ReadFromJsonAsync<CreateBudgetResponse>();
        
        //Assert
        result.Should().NotBeNull();
        result!.Budget.Id.Should().NotBe(default);
    }
    
    [Fact]
    public async Task Create_ShouldReturnCorrectResponse_WhenCorrectRequest()
    {
        //Arrange
        var command = BudgetsTestsData.CorrectCreateCommand;
        var expected = new CreateBudgetResponse
        {
            Budget = BudgetsTestsData.DefaultEntity.Adapt<BudgetDto>()
        };

        //Act
        var response = await _client.PostAsJsonAsync(PathPrefix, command);
        var result = await response.Content.ReadFromJsonAsync<CreateBudgetResponse>();
        
        //Assert
        result.Should().NotBeNull();
        result!.Should().BeEquivalentTo(expected, x=> x.Excluding(y =>  y.Budget.Id));
    }
    
    [Fact]
    public async Task Create_ShouldAddToDb_WhenCorrectRequest()
    {
        //Arrange
        var command = BudgetsTestsData.CorrectCreateCommand;
        var expected = BudgetsTestsData.DefaultEntity;
        
        //Act
        var response = await _client.PostAsJsonAsync(PathPrefix, command);
        var result = await response.Content.ReadFromJsonAsync<CreateBudgetResponse>();
        var entity = await _testDatabase.FindAsync<Budget, int>(result?.Budget.Id ?? default);

        
        //Assert
        entity.Should().BeEquivalentTo(expected, x => x.Excluding(y => y.Id).Excluding(y => y.Owner));
    }
    
    [Theory]
    [InlineData("")]
    public async Task Create_ShouldReturnBadRequest_IncorrectRequestData(string name)
    {
        //Arrange
        var command = new CreateBudgetCommand()
        {
            Name = name,
        };
        
        //Act
        var response = await _client.PostAsJsonAsync(PathPrefix, command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Create_ShouldReturnUnauthorized_WhenUserIdIsEmpty()
    {
        //Arrange
        var command = BudgetsTestsData.CorrectCreateCommand;
        _currentUserService.UserId.ReturnsNull();
        
        //Act
        var response = await _client.PostAsJsonAsync(PathPrefix, command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenUserIdIsNotInDb()
    {
        //Arrange
        var command = BudgetsTestsData.CorrectCreateCommand;
        _currentUserService.UserId.Returns("differentUserId");
        
        //Act
        var response = await _client.PostAsJsonAsync(PathPrefix, command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenBudgetAlreadyExists()
    {
        //Arrange
        var command = BudgetsTestsData.CorrectCreateCommand;
        await _testDatabase.AddAsync<Budget, int>(BudgetsTestsData.DefaultEntity);
        
        //Act
        var response = await _client.PostAsJsonAsync(PathPrefix, command);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    private void PrepareData()
    {
        var fixture = new Fixture();
        _initialUsers = new List<User>()
        {
            new() { Id = fixture.Create<string>(), FullName = "John Doe", Email = "john.doe@example.com" },
            new() { Id = fixture.Create<string>(), FullName = "Jane Doe", Email = "jane.doe@example.com" },
        };
    }
}