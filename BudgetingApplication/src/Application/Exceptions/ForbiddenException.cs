using System.Net;

namespace Application.Exceptions;

public class ForbiddenException : StatusCodeException
{
    public ForbiddenException(string message) : base(HttpStatusCode.Forbidden ,message) { }
}