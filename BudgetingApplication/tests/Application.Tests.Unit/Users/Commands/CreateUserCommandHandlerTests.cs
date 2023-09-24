using Application.Abstractions;
using Application.Abstractions.Persistence;
using Application.Exceptions;
using Application.Users.Commands.CreateUser;
using AutoFixture;
using Domain.Entities;
using NSubstitute.ReturnsExtensions;

namespace Application.Tests.Unit.Users.Commands;

public class CreateUserCommandHandlerTests
{
    private readonly CreateUserCommandHandler _sut;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUsersRepository _usersRepository;
    private readonly Fixture _fixture;

    public CreateUserCommandHandlerTests()
    {
        _currentUserService = Substitute.For<ICurrentUserService>();
        _usersRepository = Substitute.For<IUsersRepository>();
        _sut = new CreateUserCommandHandler(_currentUserService, _usersRepository);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task ShouldCallCreateWithCorrectData()
    {
        //Arrange
        var command = _fixture.Create<CreateUserCommand>();
        var userId = _fixture.Create<string>();
        _currentUserService.UserId.Returns(userId);
        
        //Act

        await _sut.Handle(command, CancellationToken.None);

        //Assert
        await _usersRepository.Received(1).Create(Arg.Is<User>(x =>
            x.Id == userId && x.FullName == command.FullName && x.Email == command.Email));
    }
    
    [Fact]
    public async Task ShouldThrowUnauthorizedException_WhenUserIdIsNull()
    {
        //Arrange
        var command = _fixture.Create<CreateUserCommand>();
        _currentUserService.UserId.ReturnsNull();
        
        //Act

         var act = () => _sut.Handle(command, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
    }
    
    [Fact]
    public async Task ShouldThrowBadRequestException_WhenUserExists()
    {
        //Arrange
        var command = _fixture.Create<CreateUserCommand>();
        var user = new User();
        
        var userId = _fixture.Create<string>();
        _currentUserService.UserId.Returns(userId);
        _usersRepository.GetById(userId, Arg.Any<CancellationToken>()).Returns(user);
        
        //Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }
}