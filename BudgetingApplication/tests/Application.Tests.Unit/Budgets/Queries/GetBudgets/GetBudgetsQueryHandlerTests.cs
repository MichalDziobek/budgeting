using Application.Abstractions;
using Application.Abstractions.Persistence;
using Application.Budgets.DataModels;
using Application.Budgets.Queries.GetBudgets;
using Application.Exceptions;
using Application.Tests.Unit.Extensions;
using AutoFixture;
using Domain.Entities;
using Mapster;
using NSubstitute.ReturnsExtensions;

namespace Application.Tests.Unit.Budgets.Queries.GetBudgets;

public class GetBudgetsQueryHandlerTests
{
    private readonly GetBudgetsQueryHandler _sut;
    private readonly ICurrentUserService _currentUserService;
    private readonly List<Budget> _users;
    private const string UserId = "userId";

    public GetBudgetsQueryHandlerTests()
    {
        _currentUserService = Substitute.For<ICurrentUserService>();
        var budgetsRepository = Substitute.For<IBudgetsRepository>();

        var fixture = new Fixture().ChangeToOmitOnRecursionBehaviour();
        _users = fixture.CreateMany<Budget>(5).ToList();
        _users[0].OwnerId = UserId;
        _users[2].OwnerId = UserId;
        
        budgetsRepository.MockGetCollection(_users);
        
        _sut = new GetBudgetsQueryHandler(budgetsRepository, _currentUserService);
    }

    [Fact]
    public async Task ShouldReturnAllResults_OnEmptyQuery()
    {
        //Arrange
        var expectedResponse = new GetBudgetsResponse
        {
            OwnedBudgets = _users.Where(x => x.OwnerId == UserId).Adapt<IEnumerable<BudgetDto>>(),
            SharedBudgets =  _users.Where(x => x.SharedBudgets.Any(y => y.UserId == UserId)).Adapt<IEnumerable<BudgetDto>>()
        };
        var query = new GetBudgetsQuery();
        _currentUserService.UserId.Returns(UserId);

        //Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        //Assert
        result.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public async Task ShouldThrowUnauthorizedException_WhenUserIdIsNull()
    {
        //Arrange
        var query = new GetBudgetsQuery();
        _currentUserService.UserId.ReturnsNull();
        
        //Act

        var act = () => _sut.Handle(query, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
    }

}