namespace FU.API.Middleware
{
    using FU.API.Exceptions;
    using Microsoft.AspNetCore.Diagnostics;

    public static class ExceptionHandler
    {
        // Handles exception by modifying the context, or lets the exceptions pass through
        public static async Task HandleException(HttpContext context)
        {
            // Get the error from the context
            var error = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

            // if exception has a response
            if (error is ExceptionWithResponse responseException)
            {
                // Update the current response's status code to use the specified one in exception
                context.Response.StatusCode = (int)responseException.StatusCode;
                // Populate the response body with the exception details
                await context.Response.WriteAsJsonAsync(responseException.GetProblemDetails());
            }
        }
    }
}