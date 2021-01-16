using EntityFramework.ErrorHandler;
using EntityFramework.ErrorHandler.PostgreSQL;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Users
{
    public static class ConfigureServices
    {
        public static void AddUsersContext(this IServiceCollection services)
        {
            services.AddDbContext<UserDbContext>(builder =>
            {
                builder.UseNpgsql("Server = 127.0.0.1; Port = 5432; Database = easyidentity.user; User Id = postgres; Password = root"); //TODO read from configuration
                DbErrorHandlerConfiguration.Config.AddPostgres();
            });
            services.AddMediatR(typeof(ConfigureServices));
            services.AddControllers();
            services.AddTransient<IPasswordHasher, PasswordHasher>();
        }

       public static void CreateAndMigrateUserDatabase(this IApplicationBuilder app)
        {
            var dbContext = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider.GetService<UserDbContext>();
            dbContext?.Database.Migrate();
        }

        public static void AddUsersContext(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllers();
        }
    }
}