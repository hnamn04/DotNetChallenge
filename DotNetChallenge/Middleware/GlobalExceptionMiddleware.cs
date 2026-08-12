using DotNetChallenge.Exceptions;
using System.Net;

namespace DotNetChallenge.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = exception switch
            {
                DuplicatePhoneException =>
                    (int)HttpStatusCode.Conflict,

                _ =>
                    (int)HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = statusCode;

            var response = new
            {
                success = false,
                message = exception.Message,
                data = (object?)null
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
