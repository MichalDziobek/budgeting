using Application.Abstractions;
using Application.Abstractions.Persistance;
using Application.Exceptions;
using Application.Users.Commands;
using AutoFixture;
using Domain.Entities;
using NSubstitute.ReturnsExtensions;

namespace Application.Tests.Unit.Users.Commands;

public class CreateUserCommandHandlerTests
{
    private readonly CreateUserCommandHandler _sut;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUsersRepository _usersRepository;

    public CreateUserCommandHandlerTests()
    {
        _currentUserService = Substitute.For<ICurrentUserService>();
        _usersRepository = Substitute.For<IUsersRepository>();
        _sut = new CreateUserCommandHandler(_currentUserService, _usersRepository);
    }

    [Fact]
    public async Task ShouldCallCreateWithCorrectData()
    {
        //Arrange
        var fixture = new Fixture();
        var command = fixture.Create<CreateUserCommand>();
        var userId = fixture.Create<string>();
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
        var fixture = new Fixture();
        var command = fixture.Create<CreateUserCommand>();
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
        var fixture = new Fixture();
        var command = fixture.Create<CreateUserCommand>();
        var user = new User();
        
        var userId = fixture.Create<string>();
        _currentUserService.UserId.Returns(userId);
        _usersRepository.GetById(userId, Arg.Any<CancellationToken>()).Returns(user);
        
        //Act
        var act = () => _sut.Handle(command, CancellationToken.None);

        //Assert
        await act.Should().ThrowAsync<BadRequestException>();
    }
}