using System.Net;

namespace Application.Exceptions;

public class UnauthorizedException : StatusCodeException
{
    public UnauthorizedException(string message) : base(HttpStatusCode.Unauthorized ,message) { }
}