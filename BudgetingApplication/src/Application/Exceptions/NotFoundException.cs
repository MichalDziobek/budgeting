using System.Net;

namespace Application.Exceptions;

public class NotFoundException : StatusCodeException
{
    public NotFoundException(string message) : base(HttpStatusCode.NotFound ,message) { }
}