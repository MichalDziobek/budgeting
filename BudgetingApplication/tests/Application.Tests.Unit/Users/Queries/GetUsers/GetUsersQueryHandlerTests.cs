using Application.Abstractions.Persistence;
using Application.Tests.Unit.Extensions;
using Application.Users.DataModels;
using Application.Users.Queries.GetUsers;
using AutoFixture;
using Domain.Entities;
using Mapster;

namespace Application.Tests.Unit.Users.Queries.GetUsers;

public class GetUsersQueryHandlerTests
{
    private readonly GetUsersQueryHandler _sut;
    private readonly List<User> _users;

    public GetUsersQueryHandlerTests()
    {
        var usersRepository = Substitute.For<IUsersRepository>();

        var fixture = new Fixture().ChangeToOmitOnRecursionBehaviour();
        _users = fixture.CreateMany<User>(5).ToList();
        
        usersRepository.MockGetCollection(_users);
        
        _sut = new GetUsersQueryHandler(usersRepository);
    }

    [Fact]
    public async Task ShouldReturnAllResults_WhenEmptyQuery()
    {
        //Arrange
        var expectedUsers = _users.Adapt<IEnumerable<UserDto>>();
        var expectedResponse = new GetUsersResponse()
        {
            Users = expectedUsers
        };
        var query = new GetUsersQuery();

        //Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        //Assert
        result.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    public async Task ShouldReturnFilteredUsers_WhenQueryWithNameFilter(int userIndex)
    {
        //Arrange
        var fullName = _users[userIndex].FullName;
        var nameSearchQuery = fullName[..(fullName.Length/2)];
        var expectedUsers = _users.Where(x => x.FullName.Contains(nameSearchQuery)).Adapt<IEnumerable<UserDto>>();
        var expectedResponse = new GetUsersResponse()
        {
            Users = expectedUsers
        };
        var query = new GetUsersQuery()
        {
            FullNameSearchQuery = nameSearchQuery
        };

        //Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        //Assert
        result.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    public async Task ShouldReturnFilteredUsers_WhenQueryWithEmailFilter(int userIndex)
    {
        //Arrange
        var email = _users[userIndex].Email;
        var emailSearchQuery = email[..(email.Length/2)];
        var expectedUsers = _users.Where(x => x.Email.Contains(emailSearchQuery)).Adapt<IEnumerable<UserDto>>();
        var expectedResponse = new GetUsersResponse()
        {
            Users = expectedUsers
        };
        var query = new GetUsersQuery()
        {
            EmailSearchQuery = emailSearchQuery
        };

        //Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        //Assert
        result.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    public async Task ShouldReturnFilteredUsers_WhenQueryWithAllFilters(int userIndex)
    {
        //Arrange
        var email = _users[userIndex].Email;
        var emailSearchQuery = email[..(email.Length/2)];
        var fullName = _users[userIndex].FullName;
        var nameSearchQuery = fullName[..(fullName.Length/2)];
        
        var expectedUsers = _users.Where(x => x.Email.Contains(emailSearchQuery) && x.FullName.Contains(nameSearchQuery)).Adapt<IEnumerable<UserDto>>();
        var expectedResponse = new GetUsersResponse()
        {
            Users = expectedUsers
        };
        var query = new GetUsersQuery()
        {
            EmailSearchQuery = emailSearchQuery,
            FullNameSearchQuery = nameSearchQuery
        };

        //Act
        var result = await _sut.Handle(query, CancellationToken.None);
        
        //Assert
        result.Should().BeEquivalentTo(expectedResponse);
    }
}