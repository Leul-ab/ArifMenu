using System.Net;
using System.Text.Json;

namespace ArifMenu.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                // Handle known status codes if no exception was thrown
                switch (context.Response.StatusCode)
                {
                    case 401:
                        await WriteJsonResponse(context, 401, "Unauthorized access.");
                        break;
                    case 403:
                        await WriteJsonResponse(context, 403, "Access forbidden.");
                        break;
                    case 404:
                        await WriteJsonResponse(context, 404, "Resource not found.");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await WriteJsonResponse(context, 500, "An internal server error occurred.");
            }
        }

        private static async Task WriteJsonResponse(HttpContext context, int statusCode, string message)
        {
            if (context.Response.HasStarted) return;

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new
            {
                status = "error",
                message = message
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
