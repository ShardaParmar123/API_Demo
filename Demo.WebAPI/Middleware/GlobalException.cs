using Demo.Contract.Services.Shared;
using Demo.Types.Dtos.Shared;
using System.Net;
using System.Text.Json;

namespace Demo.WebAPI.Middleware
{
    public class GlobalException
    {
        private readonly RequestDelegate _next;
        private static IConfiguration _config;
        private readonly ILoggerService _logger;
        private readonly IHostEnvironment _env;
        public GlobalException(RequestDelegate next, IConfiguration config, ILoggerService logger, IHostEnvironment env)
        {
            _next = next;
            _config = config;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var response = _env.IsDevelopment() ?
                    new DemoException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString()) :
                    new DemoException(context.Response.StatusCode, "Internal Server Error");
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var json = JsonSerializer.Serialize(options);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
