using System.Net;

namespace Application.Exceptions;

public class BadRequestException : StatusCodeException
{
    public BadRequestException(string message) : base(HttpStatusCode.BadRequest ,message) { }
}