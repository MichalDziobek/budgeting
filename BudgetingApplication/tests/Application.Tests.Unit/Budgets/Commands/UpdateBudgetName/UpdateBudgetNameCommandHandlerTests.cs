using Application.Abstractions;
using Application.Abstractions.Persistence;
using Application.Budgets.Commands.UpdateBudgetNameCommand;
using Application.Exceptions;
using Application.Tests.Unit.Extensions;
using AutoFixture;
using Domain.Entities;
using NSubstitute.ReturnsExtensions;

namespace Application.Tests.Unit.Budgets.Commands.UpdateBudgetName;

public class UpdateBudgetNameCommandHandlerTests
{
    private readonly UpdateBudgetNameCommandHandler _sut;
    private readonly ICurrentUserService _currentUserService;
    private readonly IBudgetsRepository _budgetsRepository;

    public UpdateBudgetNameCommandHandlerTests()
    {
        _currentUserService = Substitute.For<ICurrentUserService>();
        _budgetsRepository = Substitute.For<IBudgetsRepository>();
        _sut = new UpdateBudgetNameCommandHandler(_currentUserService, _budgetsRepository);
    }

    [Fact]
    public async Task ShouldCallUpdateWithCorrectData()
    {
        //Arrange
        var fixture = new Fixture().ChangeToOmitOnRecursionBehaviour();
        var command = fixture.Create<UpdateBudgetNameCommand>();
        var userId = fixture.Create<string>();
        var budget = fixture.Create<Budget>();
        budget.OwnerId = userId;
        budget.Id = command.BudgetId;

        _currentUserService.UserId.Returns(userId);
        _budgetsRepository.GetById(command.BudgetId, Arg.Any<CancellationToken>()).Returns(budget);
        
        //Act

        await _sut.Handle(command, CancellationToken.None);

        //Assert
        await _budgetsRepository.Received(1).Update(Arg.Is<Budget>(x =>
            x.Id == command.BudgetId && x.Name == command.Name));
    }
    
    [Fact]
    public async Task ShouldThrowUnauthorizedException_WhenUserIdIsNull()
    {
        //Arrange
        var fixture = new Fixture();
        var command = fixture.Create<UpdateBudgetNameCommand>();
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
        var fixture = new Fixture();
        var command = fixture.Create<UpdateBudgetNameCommand>();
        var userId = fixture.Create<string>();
        
        _currentUserService.UserId.Returns(userId);
        
        //Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
    
    [Fact]
    public async Task ShouldThrowForbiddenException_WhenBudgetOwnerIdDoesNotMatchUserId()
    {
        //Arrange
        var fixture = new Fixture().ChangeToOmitOnRecursionBehaviour();
        var command = fixture.Create<UpdateBudgetNameCommand>();
        var userId = fixture.Create<string>();
        var budget = fixture.Create<Budget>();

        _budgetsRepository.GetById(command.BudgetId, Arg.Any<CancellationToken>()).Returns(budget);
        _currentUserService.UserId.Returns(userId);
        
        //Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }
}