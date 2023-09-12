using Demo.Contract.Services.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace Demo.WebAPI.Middleware
{
    public class Authorization
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerService _logger;        

        public Authorization(
            RequestDelegate next,
            ILoggerService logger)
        {
            _next = next;
            _logger = logger;
        }
    }
}

