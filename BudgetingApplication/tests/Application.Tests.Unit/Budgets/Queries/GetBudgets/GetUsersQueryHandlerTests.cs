using Application.Abstractions;
using Application.Abstractions.Persistence;
using Application.Budgets.Commands.CreateBudget;
using Application.Budgets.Queries.GetBudgets;
using Application.Exceptions;
using Application.Tests.Unit.Extensions;
using Application.Users.Queries.DataModels;
using Application.Users.Queries.GetUsers;
using AutoFixture;
using Domain.Entities;
using Mapster;
using NSubstitute.ReturnsExtensions;

namespace Application.Tests.Unit.Users.Queries.GetUsers;

public class GetBudgetsQueryHandlerTests
{
    private readonly GetBudgetsQueryHandler _sut;
    private readonly ICurrentUserService _currentUserService;
    private readonly List<Budget> _users;
    private const string userId = "userId";

    public GetBudgetsQueryHandlerTests()
    {
        _currentUserService = Substitute.For<ICurrentUserService>();
        var budgetsRepository = Substitute.For<IBudgetsRepository>();

        var fixture = new Fixture().ChangeToOmitOnRecursionBehaviour();
        _users = fixture.CreateMany<Budget>(5).ToList();
        _users[0].OwnerId = userId;
        _users[2].OwnerId = userId;
        
        budgetsRepository.MockGetCollection(_users);
        
        _sut = new GetBudgetsQueryHandler(budgetsRepository, _currentUserService);
    }

    [Fact]
    public async Task ShouldReturnAllResults_OnEmptyQuery()
    {
        //Arrange
        var expectedResponse = new GetBudgetsResponse
        {
            OwnedBudgets = _users.Where(x => x.OwnerId == userId).Adapt<IEnumerable<BudgetDto>>(),
            SharedBudgets =  _users.Where(x => x.UsersWithSharedAccess.Any(y => y.Id == userId)).Adapt<IEnumerable<BudgetDto>>()
        };
        var query = new GetBudgetsQuery();
        _currentUserService.UserId.Returns(userId);

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