using Application.Abstractions;
using Application.Abstractions.Persistence;
using Application.Budgets.Commands.CreateBudget;
using Application.Exceptions;
using Application.Tests.Unit.Extensions;
using AutoFixture;
using Domain.Entities;
using Mapster;
using NSubstitute.ReturnsExtensions;

namespace Application.Tests.Unit.Budgets.Commands.CreateBudget;

public class CreateBudgetCommandHandlerTests
{
    private readonly CreateBudgetCommandHandler _sut;
    private readonly ICurrentUserService _currentUserService;
    private readonly IBudgetsRepository _budgetsRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly Fixture _fixture;

    public CreateBudgetCommandHandlerTests()
    {
        _currentUserService = Substitute.For<ICurrentUserService>();
        _budgetsRepository = Substitute.For<IBudgetsRepository>();
        _usersRepository = Substitute.For<IUsersRepository>();
        _sut = new CreateBudgetCommandHandler(_currentUserService, _budgetsRepository, _usersRepository);
        _fixture = new Fixture().ChangeToOmitOnRecursionBehaviour();
    }

    [Fact]
    public async Task ShouldCallCreateWithCorrectData()
    {
        //Arrange
        var command = _fixture.Create<CreateBudgetCommand>();
        var user = _fixture.Create<User>();
        var userId = user.Id;
        _currentUserService.UserId.Returns(userId);
        _usersRepository.MockExists(new[] { user });
        
        //Act

        await _sut.Handle(command, CancellationToken.None);

        //Assert
        await _budgetsRepository.Received(1).Create(Arg.Is<Budget>(x =>
            x.OwnerId == userId && x.Name == command.Name));
    }
    
    [Fact]
    public async Task ShouldReturnCorrectResult()
    {
        //Arrange
        var command = _fixture.Create<CreateBudgetCommand>();
        var user = _fixture.Create<User>();
        var userId = user.Id;
        _currentUserService.UserId.Returns(userId);
        _usersRepository.MockExists(new[] { user });
        var budget = _fixture.Create<Budget>();

        _budgetsRepository.Create(Arg.Is<Budget>(x => x.OwnerId == userId && x.Name == command.Name),
            Arg.Any<CancellationToken>()).Returns(budget);
        
        //Act

        var result = await _sut.Handle(command, CancellationToken.None);

        //Assert
        result.Budget.Id.Should().NotBe(default);
    }
    
    [Fact]
    public async Task ShouldThrowUnauthorizedException_WhenUserIdIsNull()
    {
        //Arrange
        var command = _fixture.Create<CreateBudgetCommand>();
        _currentUserService.UserId.ReturnsNull();
        
        //Act
         var act = () => _sut.Handle(command, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
    }
    
    [Fact]
    public async Task ShouldReturnThrowBadRequestException_WhenUserDoesNotExist()
    {
        //Arrange
        var command = _fixture.Create<CreateBudgetCommand>();
        var user = _fixture.Create<User>();
        var userId = user.Id;
        _currentUserService.UserId.Returns(userId);
        _usersRepository.MockExists(Enumerable.Empty<User>());
        var budget = _fixture.Create<Budget>();

        _budgetsRepository.Create(Arg.Is<Budget>(x => x.OwnerId == userId && x.Name == command.Name),
            Arg.Any<CancellationToken>()).Returns(budget);
        
        //Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }
    
    [Fact]
    public async Task ShouldThrowBadRequestException_WhenBudgetExistsExists()
    {
        //Arrange
        var command = _fixture.Create<CreateBudgetCommand>();
        var budget = command.Adapt<Budget>();
        var userId = _fixture.Create<string>();
        budget.OwnerId = userId;
        
        _currentUserService.UserId.Returns(userId);
        _budgetsRepository.MockExists(new []{ budget });
        
        //Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }
}