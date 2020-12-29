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
            services.AddDbContext<UserDbContext>();
            services.AddMediatR(typeof(ConfigureServices));
            services.AddControllers();
            services.AddTransient<IPasswordHasher, PasswordHasher>();
        }

        public static void AddUsersContext(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllers();
        }
    }
}