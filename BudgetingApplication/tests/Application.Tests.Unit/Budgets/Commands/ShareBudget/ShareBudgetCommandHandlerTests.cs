using Application.Abstractions;
using Application.Abstractions.Persistence;
using Application.Budgets.Commands.ShareBudget;
using Application.Exceptions;
using Application.Tests.Unit.Extensions;
using AutoFixture;
using Domain.Entities;
using NSubstitute.ReturnsExtensions;

namespace Application.Tests.Unit.Budgets.Commands.ShareBudget;

public class ShareBudgetCommandHandlerTests
{
    private readonly ShareBudgetCommandHandler _sut;
    private readonly ICurrentUserService _currentUserService;
    private readonly IBudgetsRepository _budgetsRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly Fixture _fixture;

    public ShareBudgetCommandHandlerTests()
    {
        _currentUserService = Substitute.For<ICurrentUserService>();
        _budgetsRepository = Substitute.For<IBudgetsRepository>();
        _usersRepository = Substitute.For<IUsersRepository>();
        _sut = new ShareBudgetCommandHandler(_currentUserService, _budgetsRepository, _usersRepository);
        _fixture = new Fixture().ChangeToOmitOnRecursionBehaviour();
    }

    [Fact]
    public async Task ShouldCallUpdateWithCorrectData()
    {
        //Arrange
        var sharedToUser = GetCommand(out var command);

        //Act

        await _sut.Handle(command, CancellationToken.None);

        //Assert
        await _budgetsRepository.Received(1).AddSharedBudget(Arg.Is<SharedBudget>(x =>
            x.UserId == sharedToUser.Id), Arg.Any<CancellationToken>());
    }

    private User GetCommand(out ShareBudgetCommand command)
    {
        var sharedToUser = _fixture.Create<User>();
        var currentUserId = _fixture.Create<string>();
        var budget = _fixture.Create<Budget>();
        budget.OwnerId = currentUserId;

        command = new ShareBudgetCommand
        {
            BudgetId = budget.Id,
            SharedUserId = sharedToUser.Id
        };

        _currentUserService.UserId.Returns(currentUserId);
        _usersRepository.GetById(command.SharedUserId, Arg.Any<CancellationToken>()).Returns(sharedToUser);
        _budgetsRepository.GetById(command.BudgetId, Arg.Any<CancellationToken>()).Returns(budget);
        return sharedToUser;
    }

    [Fact]
    public async Task ShouldThrowUnauthorizedException_WhenUserIdIsNull()
    {
        //Arrange
        var command = _fixture.Create<ShareBudgetCommand>();
        _currentUserService.UserId.ReturnsNull();
        
        //Act

         var act = () => _sut.Handle(command, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
    }
    
    [Fact]
    public async Task ShouldThrowNotFoundException_WhenBudgetDoesNotExist()
    {
        //Arrange
        var command = _fixture.Create<ShareBudgetCommand>();
        var currentUserId = _fixture.Create<string>();
        
        _currentUserService.UserId.Returns(currentUserId);
        
        //Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task ShouldThrowBadRequestException_WhenSharedToUserDoesNotExist()
    {
        //Arrange
        var command = _fixture.Create<ShareBudgetCommand>();
        var currentUserId = _fixture.Create<string>();
        var budget = _fixture.Create<Budget>();

        _budgetsRepository.GetById(command.BudgetId, Arg.Any<CancellationToken>()).Returns(budget);
        _usersRepository.GetById(command.SharedUserId, Arg.Any<CancellationToken>()).ReturnsNull();
        _currentUserService.UserId.Returns(currentUserId);
        
        //Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }
    
    [Fact]
    public async Task ShouldThrowForbiddenException_WhenBudgetOwnerIdDoesNotMatchUserId()
    {
        //Arrange
        var command = _fixture.Create<ShareBudgetCommand>();
        var currentUserId = _fixture.Create<string>();
        var budget = _fixture.Create<Budget>();
        var sharedToUser = _fixture.Create<User>();

        _budgetsRepository.GetById(command.BudgetId, Arg.Any<CancellationToken>()).Returns(budget);
        _usersRepository.GetById(command.SharedUserId, Arg.Any<CancellationToken>()).Returns(sharedToUser);
        _currentUserService.UserId.Returns(currentUserId);
        
        //Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }
}