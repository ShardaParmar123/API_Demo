using Microsoft.EntityFrameworkCore;

namespace Demo.Infrastructure.Configuration.ModelConfiguration.Auth
{
    public static class AuthModelConfiguration
    {
        public static void ApplyConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
