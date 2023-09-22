using Application.Abstractions;
using Application.Abstractions.Persistance;
using Application.Users.Commands;
using AutoFixture;
using Domain.Entities;

namespace Application.Tests.Unit.Users.Command;

public class CreateUserCommandTests
{
    private readonly CreateUserCommandHandler _sut;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUsersRepository _usersRepository;

    public CreateUserCommandTests()
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
}