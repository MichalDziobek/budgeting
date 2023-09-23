using Application.Abstractions;
using Application.Abstractions.Persistence;
using Application.BudgetEntries.Queries.GetBudgetEntries;
using Application.BudgetEntries.DataModel;
using Application.BudgetEntries.Queries.GetBudgetEntries;
using Application.DataModels.Common;
using Application.Exceptions;
using Application.Tests.Unit.Extensions;
using AutoFixture;
using Domain.Entities;
using Domain.Enums;
using Mapster;
using NSubstitute.ReturnsExtensions;

namespace Application.Tests.Unit.BudgetEntries.Queries.GetBudgetEntries;

public class GetBudgetEntriesQueryHandlerTests
{
    private readonly GetBudgetEntriesQueryHandler _sut;
    private readonly List<BudgetEntry> _budgetEntries;
    private readonly IBudgetsRepository _budgetsRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly Fixture _fixture;

    private readonly List<int> _categoryIds;
    private readonly int _budgetId;

    public GetBudgetEntriesQueryHandlerTests()
    {
        var budgetEntriesRepository = Substitute.For<IBudgetEntriesRepository>();
        _budgetsRepository = Substitute.For<IBudgetsRepository>();
        _currentUserService = Substitute.For<ICurrentUserService>();

        _fixture = new Fixture().ChangeToOmitOnRecursionBehaviour();
        _categoryIds = _fixture.CreateMany<int>(2).ToList();
        _budgetEntries = _fixture.CreateMany<BudgetEntry>(5).ToList();

        _budgetId = _fixture.Create<int>();
        foreach (var budgetEntry in _budgetEntries)
        {
            budgetEntry.BudgetId = _budgetId;
        }
        _budgetEntries[0].CategoryId = _categoryIds[0];
        _budgetEntries[1].CategoryId = _categoryIds[0];
        _budgetEntries[2].CategoryId = _categoryIds[1];
        _budgetEntries[3].CategoryId = _categoryIds[1];
        _budgetEntries[4].CategoryId = _categoryIds[0];
        
        budgetEntriesRepository.MockGetPaginatedCollection(_budgetEntries);
        
        _sut = new GetBudgetEntriesQueryHandler(_budgetsRepository, budgetEntriesRepository, _currentUserService);
    }

    [Fact]
    public async Task ShouldReturnAllResults_OnEmptyFiltersAndLimitExceedingMax_ForBudgetOwner()
    {
        //Arrange
        var expectedBudgetEntries = _budgetEntries.Adapt<IEnumerable<BudgetEntryDto>>();
        var expectedResponse = new GetBudgetEntriesResponse()
        {
            BudgetEntries = PaginatedResponse<BudgetEntryDto>.From(expectedBudgetEntries)
        };

        var budget = GetOwnedBudget();
        
        var query = new GetBudgetEntriesQuery
        {
            BudgetId = budget.Id,
            Offset = 0,
            Limit = _budgetEntries.Count + 1,
            CategoryFilter = null,
            BudgetEntryTypeFilter = null
        };

        //Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        //Assert
        result.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public async Task ShouldReturnAllResults_OnEmptyFiltersAndLimitExceedingMax_ForSharedBudget()
    {
        //Arrange
        var expectedBudgetEntries = _budgetEntries.Adapt<IEnumerable<BudgetEntryDto>>();
        var expectedResponse = new GetBudgetEntriesResponse()
        {
            BudgetEntries = PaginatedResponse<BudgetEntryDto>.From(expectedBudgetEntries)
        };

        var userId = _fixture.Create<string>();
        _currentUserService.UserId.Returns(userId);
        var budget = _fixture.Create<Budget>();
        budget.Id = _budgetId;
        budget.SharedBudgets = new List<SharedBudget>() { new(){ BudgetId = budget.Id, UserId = userId} };
        _budgetsRepository.GetById(budget.Id, Arg.Any<CancellationToken>()).Returns(budget);
        
        var query = new GetBudgetEntriesQuery
        {
            BudgetId = budget.Id,
            Offset = 0,
            Limit = _budgetEntries.Count + 1,
            CategoryFilter = null,
            BudgetEntryTypeFilter = null
        };

        //Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        //Assert
        result.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Theory]
    [InlineData(0,1)]
    [InlineData(2,2)]
    [InlineData(1,5)]
    public async Task ShouldReturnPaginatedResults_OnEmptyFilters(int offset, int limit)
    {
        //Arrange
        var expectedBudgetEntries = _budgetEntries.Skip(offset).Take(limit).Adapt<IEnumerable<BudgetEntryDto>>();
        var expectedResponse = new GetBudgetEntriesResponse()
        {
            BudgetEntries = PaginatedResponse<BudgetEntryDto>.From(expectedBudgetEntries)
        };

        var budget = GetOwnedBudget();

        var query = new GetBudgetEntriesQuery
        {
            BudgetId = budget.Id,
            Offset = offset,
            Limit = limit,
            CategoryFilter = null,
            BudgetEntryTypeFilter = null
        };

        //Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        //Assert
        result.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Theory]
    [InlineData(0,1, 0)]
    [InlineData(2,2, 0)]
    [InlineData(1,5, 0)]
    [InlineData(0,1, 1)]
    [InlineData(2,2, 1)]
    [InlineData(1,5, 1)]
    public async Task ShouldReturnPaginatedResults_OnCategoryFilters(int offset, int limit, int categoryIdIndex)
    {
        //Arrange
        var expectedBudgetEntries = _budgetEntries.Where(x => x.CategoryId == _categoryIds[categoryIdIndex])
            .Skip(offset).Take(limit).Adapt<IEnumerable<BudgetEntryDto>>();
        var expectedResponse = new GetBudgetEntriesResponse()
        {
            BudgetEntries = PaginatedResponse<BudgetEntryDto>.From(expectedBudgetEntries)
        };

        var budget = GetOwnedBudget();

        var query = new GetBudgetEntriesQuery
        {
            BudgetId = budget.Id,
            Offset = offset,
            Limit = limit,
            CategoryFilter = _categoryIds[categoryIdIndex],
            BudgetEntryTypeFilter = null
        };

        //Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        //Assert
        result.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Theory]
    [InlineData(0,1, 0)]
    [InlineData(2,2, 0)]
    [InlineData(1,5, 0)]
    [InlineData(0,1, 1)]
    [InlineData(2,2, 1)]
    [InlineData(1,5, 1)]
    public async Task ShouldReturnPaginatedResults_OnBudgetEntryTypeFilters(int offset, int limit, int entryType)
    {
        //Arrange
        var budgetEntryType = (BudgetEntryType)entryType;
        var budgetEntries = budgetEntryType switch
        {
            BudgetEntryType.Income => _budgetEntries.Where(x => x.Value > 0),
            BudgetEntryType.Expense => _budgetEntries.Where(x => x.Value < 0),
            _ => throw new ArgumentOutOfRangeException()
        };
        var expectedBudgetEntries = budgetEntries.Skip(offset).Take(limit).Adapt<IEnumerable<BudgetEntryDto>>();
        var expectedResponse = new GetBudgetEntriesResponse()
        {
            BudgetEntries = PaginatedResponse<BudgetEntryDto>.From(expectedBudgetEntries)
        };

        var budget = GetOwnedBudget();

        var query = new GetBudgetEntriesQuery
        {
            BudgetId = budget.Id,
            Offset = offset,
            Limit = limit,
            CategoryFilter = null,
            BudgetEntryTypeFilter = budgetEntryType
        };

        //Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        //Assert
        result.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public async Task ShouldThrowNotFoundException_OnNonExistingBudgetId()
    {
        //Arrange
        _ = GetOwnedBudget();

        var query = new GetBudgetEntriesQuery
        {
            BudgetId = 0,
            Offset = 0,
            Limit = _budgetEntries.Count + 1,
            CategoryFilter = null,
            BudgetEntryTypeFilter = null
        };

        //Act
        var act = () => _sut.Handle(query, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task ShouldThrowUnauthorizedException_OnNullUserId()
    {
        //Arrange
        _ = GetOwnedBudget();
        _currentUserService.UserId.ReturnsNull();

        var query = new GetBudgetEntriesQuery
        {
            BudgetId = 0,
            Offset = 0,
            Limit = _budgetEntries.Count + 1,
            CategoryFilter = null,
            BudgetEntryTypeFilter = null
        };

        //Act
        var act = () => _sut.Handle(query, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
    }
    
    [Fact]
    public async Task ShouldThrowForbiddenException_OnNullUserId()
    {
        //Arrange
        var budget = GetOwnedBudget();
        _currentUserService.UserId.Returns("DifferentUserId");

        var query = new GetBudgetEntriesQuery
        {
            BudgetId = budget.Id,
            Offset = 0,
            Limit = _budgetEntries.Count + 1,
            CategoryFilter = null,
            BudgetEntryTypeFilter = null
        };

        //Act
        var act = () => _sut.Handle(query, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    private Budget GetOwnedBudget()
    {
        var userId = _fixture.Create<string>();
        _currentUserService.UserId.Returns(userId);
        var budget = _fixture.Create<Budget>();
        budget.Id = _budgetId;
        budget.OwnerId = userId;
        _budgetsRepository.GetById(budget.Id, Arg.Any<CancellationToken>()).Returns(budget);
        return budget;
    }
}