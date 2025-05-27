using Microsoft.AspNetCore.Http;
using PlanyApp.API.Models;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.ComponentModel.DataAnnotations;

namespace PlanyApp.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                // Handle 401 Unauthorized
                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    await HandleUnauthorizedResponse(context);
                }
                // Handle 403 Forbidden
                else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    await HandleForbiddenResponse(context);
                }
                // Handle 404 Not Found
                else if (context.Response.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    await HandleNotFoundResponse(context);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleUnauthorizedResponse(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            var response = ApiResponse<object>.ErrorResponse(
                "Authentication failed. Please login or provide valid credentials.",
                new { error = "Unauthorized access" }
            );
            await context.Response.WriteAsJsonAsync(response);
        }

        private async Task HandleForbiddenResponse(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            var response = ApiResponse<object>.ErrorResponse(
                "You don't have permission to access this resource.",
                new { error = "Access forbidden" }
            );
            await context.Response.WriteAsJsonAsync(response);
        }

        private async Task HandleNotFoundResponse(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            var response = ApiResponse<object>.ErrorResponse(
                "The requested resource was not found.",
                new { error = "Resource not found" }
            );
            await context.Response.WriteAsJsonAsync(response);
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = ApiResponse<object>.ErrorResponse(
                _env.IsDevelopment() ? exception.Message : "An internal server error occurred.",
                _env.IsDevelopment() ? new 
                { 
                    error = exception.Message,
                    stackTrace = exception.StackTrace
                } : new { error = "An internal server error occurred" }
            );

            switch (exception)
            {
                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Message = "Authentication failed.";
                    break;

                case InvalidCredentialException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Message = "Invalid credentials provided.";
                    break;

                case KeyNotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = "The requested resource was not found.";
                    break;

                case ValidationException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = "Validation failed.";
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            await context.Response.WriteAsJsonAsync(response);
        }
    }

    // Extension method to make it easier to add the middleware
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
} 