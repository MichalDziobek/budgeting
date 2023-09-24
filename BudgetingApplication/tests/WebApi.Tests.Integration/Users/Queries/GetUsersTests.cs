using System.Net;
using System.Net.Http.Json;
using Application.Users.DataModels;
using Application.Users.Queries.GetUsers;
using AutoFixture;
using Domain.Entities;
using FluentAssertions;
using Mapster;
using Microsoft.AspNetCore.WebUtilities;
using WebApi.Tests.Integration.Common;
using WebApi.Tests.Integration.Common.Abstractions;
using Xunit;

namespace WebApi.Tests.Integration.Users.Queries;

[Collection(nameof(SharedTestCollection))]
public class GetUsersTests : IAsyncLifetime
{
    private const string PathPrefix = "users";
    private readonly HttpClient _client;
    private readonly ITestDatabase _testDatabase;
    private List<User> _initialUsers = new();

    public GetUsersTests(CustomWebApplicationFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
        _testDatabase = apiFactory.GetTestDatabase();

        PrepareData();
    }


    public async Task InitializeAsync() => await _testDatabase.AddRangeAsync<User, string>(_initialUsers);

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
    
    [Fact]
    public async Task Get_ShouldReturnAllUsers_OnEmptyQuery()
    {
        //Arrange
        var queryParams = new Dictionary<string, string?>()
        {
            { nameof(GetUsersQuery.FullNameSearchQuery), null },
            { nameof(GetUsersQuery.EmailSearchQuery), null }
        };
        var expectedResult = new GetUsersResponse
        {
            Users = _initialUsers.Adapt<IEnumerable<UserDto>>()
        };
        var url = QueryHelpers.AddQueryString(PathPrefix, queryParams);

        //Act
        var response = await _client.GetAsync(url);
        var result = await response.Content.ReadFromJsonAsync<GetUsersResponse>();
        
        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
    
    [Theory]
    [InlineData("Jane")]
    [InlineData("Smith")]
    [InlineData("John Doe")]
    public async Task Get_ShouldReturnFilteredUsers_OnNameQuery(string nameQuery)
    {
        //Arrange
        var queryParams = new Dictionary<string, string?>()
        {
            { nameof(GetUsersQuery.FullNameSearchQuery), nameQuery },
            { nameof(GetUsersQuery.EmailSearchQuery), null }
        };
        var expectedResult = new GetUsersResponse
        {
            Users = _initialUsers.Where(x => x.FullName.Contains(nameQuery)).Adapt<IEnumerable<UserDto>>()
        };

        var url = QueryHelpers.AddQueryString(PathPrefix, queryParams);


        //Act
        var response = await _client.GetAsync(url);
        var result = await response.Content.ReadFromJsonAsync<GetUsersResponse>();
        
        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
    
    [Theory]
    [InlineData("jane")]
    [InlineData("smith")]
    [InlineData("john.doe@example.com")]
    public async Task Get_ShouldReturnFilteredUsers_OnEmailQuery(string emailQuery)
    {
        //Arrange
        var queryParams = new Dictionary<string, string?>()
        {
            { nameof(GetUsersQuery.FullNameSearchQuery), null },
            { nameof(GetUsersQuery.EmailSearchQuery), emailQuery }
        };
        var expectedResult = new GetUsersResponse
        {
            Users = _initialUsers.Where(x => x.Email.Contains(emailQuery)).Adapt<IEnumerable<UserDto>>()
        };

        var url = QueryHelpers.AddQueryString(PathPrefix, queryParams);


        //Act
        var response = await _client.GetAsync(url);
        var result = await response.Content.ReadFromJsonAsync<GetUsersResponse>();
        
        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
    
    [Theory]
    [InlineData("Jane", "jane")]
    [InlineData("Smith", "smith")]
    [InlineData("John Doe", "john.doe@example.com")]
    public async Task Get_ShouldReturnFilteredUsers_OnEmailAndNameQuery(string nameQuery, string emailQuery)
    {
        //Arrange
        var queryParams = new Dictionary<string, string?>()
        {
            { nameof(GetUsersQuery.FullNameSearchQuery), nameQuery },
            { nameof(GetUsersQuery.EmailSearchQuery), emailQuery }
        };
        var expectedResult = new GetUsersResponse
        {
            Users = _initialUsers.Where(x => x.Email.Contains(emailQuery) && x.FullName.Contains(nameQuery)).Adapt<IEnumerable<UserDto>>()
        };

        var url = QueryHelpers.AddQueryString(PathPrefix, queryParams);


        //Act
        var response = await _client.GetAsync(url);
        var result = await response.Content.ReadFromJsonAsync<GetUsersResponse>();
        
        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
    
    
    private void PrepareData()
    {
        var fixture = new Fixture();
        _initialUsers = new List<User>()
        {
            new() { Id = fixture.Create<string>(), FullName = "John Doe", Email = "john.doe@example.com" },
            new() { Id = fixture.Create<string>(), FullName = "Jane Doe", Email = "jane.doe@example.com" },
            new() { Id = fixture.Create<string>(), FullName = "Jim Doe", Email = "jim.doe@example.com" },
            new() { Id = fixture.Create<string>(), FullName = "John Smith", Email = "john.smith@example.com" },
            new() { Id = fixture.Create<string>(), FullName = "Jane Smith", Email = "jane.smith@example.com" },
        };
    }
}