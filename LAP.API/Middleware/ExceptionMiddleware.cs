namespace LAP.API.Middleware
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentValidation;
    using LAP.Shared.Exceptions;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Middleware responsible for catching and handling all exceptions thrown
    /// during the request pipeline, returning structured JSON responses.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the ASP.NET Core pipeline.</param>
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Executes the middleware, capturing and processing exceptions raised by downstream components.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context); // Attempt to process the next middleware
            }
            catch (BaseCustomException ex) // Custom application exception
            {
                await HandleCustomException(context, ex);
            }
            catch (ValidationException ex) // FluentValidation validation failure
            {
                await HandleValidationException(context, ex);
            }
            catch (Exception ex) // Any unknown or unhandled exception
            {
                await HandleUnknownException(context, ex);
            }
        }

        /// <summary>
        /// Handles all custom exceptions derived from <see cref="BaseCustomException"/>,
        /// returning a structured JSON response with the appropriate status code.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        /// <param name="ex">The custom exception thrown.</param>
        private async Task HandleCustomException(HttpContext context, BaseCustomException ex)
        {
            context.Response.ContentType = "application/json"; // Set response type
            context.Response.StatusCode = ex.StatusCode; // Apply custom status code

            await context.Response.WriteAsJsonAsync(
                new
                {
                    code = ex.StatusCode,
                    message = ex.Message,
                    description = ex.Description,
                }
            );
        }

        /// <summary>
        /// Handles FluentValidation validation failures, returning a 400 Bad Request response
        /// with the validation error details.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        /// <param name="ex">The validation exception.</param>
        private async Task HandleValidationException(HttpContext context, ValidationException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 400;

            await context.Response.WriteAsJsonAsync(
                new
                {
                    code = 400,
                    message = "Validation failed",
                    description = string.Join("; ", ex.Errors.Select(e => e.ErrorMessage)),
                }
            );
        }

        /// <summary>
        /// Handles all unknown exceptions, returning a standardized internal server error response.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        /// <param name="ex">The exception that occurred.</param>
        private async Task HandleUnknownException(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json"; // Set response format
            context.Response.StatusCode = 500; // Internal server error code

            await context.Response.WriteAsJsonAsync(
                new
                {
                    code = 500,
                    message = "Internal Server Error",
                    description = ex.Message,
                }
            );
        }
    }
}
