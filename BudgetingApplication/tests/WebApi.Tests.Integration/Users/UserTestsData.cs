using Application.Users.Commands.CreateUser;
using Domain.Entities;

namespace WebApi.Tests.Integration.Users;

public class UserTestsData
{
    public const string CorrectEmail = "correct-email@example.com";
    public const string CorrectName = "Jane Doe";
    public const string DefaultUserId = "auth0|12345";

    public static CreateUserCommand CorrectCreateUserCommand => new CreateUserCommand
    {
        Email = CorrectEmail,
        FullName = CorrectName
    };

    public static User DefaultUser => new User
    {
        Id = DefaultUserId,
        FullName = CorrectName,
        Email = CorrectEmail,
    };
}