using System.Net;
using System.Net.Http.Json;
using Application.Categories.DataModel;
using Application.Categories.Queries.GetCategories;
using AutoFixture;
using Domain.Entities;
using FluentAssertions;
using Mapster;
using Microsoft.AspNetCore.WebUtilities;
using WebApi.Tests.Integration.Common;
using WebApi.Tests.Integration.Common.Abstractions;
using Xunit;

namespace WebApi.Tests.Integration.Categories.Queries.GetCategories;

[Collection(nameof(SharedTestCollection))]
public class GetCategoriesTests : IAsyncLifetime
{
    private const string PathPrefix = "categories";
    private readonly HttpClient _client;
    private readonly ITestDatabase _testDatabase;
    private List<Category> _initialCategories = new();

    public GetCategoriesTests(CustomWebApplicationFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
        _testDatabase = apiFactory.GetTestDatabase();

        PrepareData();
    }

    public async Task InitializeAsync() => await _testDatabase.AddRangeAsync<Category, int>(_initialCategories);

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
    public async Task Get_ShouldReturnAllCategories_OnEmptyQuery()
    {
        //Arrange
        var queryParams = new Dictionary<string, string?>()
        {
            { nameof(GetCategoriesQuery.NameSearchQuery), null },
        };
        var expectedResult = new GetCategoriesResponse
        {
            Categories = _initialCategories.Adapt<IEnumerable<CategoryDto>>()
        };
        var url = QueryHelpers.AddQueryString(PathPrefix, queryParams);

        //Act
        var response = await _client.GetAsync(url);
        var result = await response.Content.ReadFromJsonAsync<GetCategoriesResponse>();
        
        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
    
    [Theory]
    [InlineData("Category")]
    [InlineData("Cat")]
    [InlineData("Other")]
    public async Task Get_ShouldReturnFilteredCategories_OnNameQuery(string nameQuery)
    {
        //Arrange
        var queryParams = new Dictionary<string, string?>()
        {
            { nameof(GetCategoriesQuery.NameSearchQuery), nameQuery },
        };
        var expectedResult = new GetCategoriesResponse
        {
            Categories = _initialCategories.Where(x => x.Name.Contains(nameQuery)).Adapt<IEnumerable<CategoryDto>>()
        };

        var url = QueryHelpers.AddQueryString(PathPrefix, queryParams);


        //Act
        var response = await _client.GetAsync(url);
        var result = await response.Content.ReadFromJsonAsync<GetCategoriesResponse>();
        
        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
    
    private void PrepareData()
    {
        var fixture = new Fixture();
        _initialCategories = new List<Category>()
        {
            new() { Id = fixture.Create<int>(), Name = "Expense - Other" },
            new() { Id = fixture.Create<int>(), Name = "Income - Other" },
            new() { Id = fixture.Create<int>(), Name = "Salary" },
            new() { Id = fixture.Create<int>(), Name = "Category 4" },
            new() { Id = fixture.Create<int>(), Name = "Category 5" },
        };
    }
}