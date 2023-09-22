using System.Net;
using FluentValidation.Results;

namespace Application.Exceptions;

public class ValidationException : StatusCodeException
{
    public ValidationException(IEnumerable<ValidationFailure> failures) 
        : base(HttpStatusCode.BadRequest, "One or more validation failures have occurred.")
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public IDictionary<string, string[]> Errors { get; }
}