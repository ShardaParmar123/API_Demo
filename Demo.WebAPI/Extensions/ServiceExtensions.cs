using Demo.BLL.Services;
using Demo.BLL.Services.Auth;
using Demo.BLL.Services.Shared;
using Demo.Contract.Repositories.Auth;
using Demo.Contract.Services.Auth;
using Demo.Contract.Services.Shared;
using Demo.DAL.Repositories.Auth;
using Demo.Infrastructure.Configuration;

namespace Demo.WebAPI.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureDAL(this WebApplicationBuilder app)
        {
            app.Services.AddScoped<DemoDbContext>();

            //Auth Repositories
            app.Services.AddScoped<IUserRepository, UserRepository>();
        }

        public static void ConfigureBLL(this WebApplicationBuilder app)
        {
            //Auth Services
            app.Services.AddScoped<IUserService, UserService>();
            app.Services.AddScoped<IAuthService, AuthService>();
        }
        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerService, LoggerService>();
        }
    }
}
