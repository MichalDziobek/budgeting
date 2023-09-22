using System.Net.Mime;
using Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace WebApi;

public static class ExceptionHandlingExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                context.Response.ContentType = MediaTypeNames.Text.Plain;

                var exceptionHandlerPathFeature =
                    context.Features.Get<IExceptionHandlerFeature>();

                if (exceptionHandlerPathFeature?.Error is null)
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync("An exception was thrown.");
                    return;
                }

                switch (exceptionHandlerPathFeature.Error)
                {
                    case ValidationException validationException:
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsJsonAsync(validationException.Errors);
                        break;
                    case StatusCodeException statusCodeException:
                        context.Response.StatusCode = (int)statusCodeException.StatusCode;
                        await context.Response.WriteAsync(statusCodeException.Message);
                        break;
                    default:
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        await context.Response.WriteAsync(exceptionHandlerPathFeature.Error.Message);
                        break;
                }
            });
        });
    }
}