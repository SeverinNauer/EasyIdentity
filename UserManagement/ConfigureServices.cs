using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace UserManagement
{
    public static class ConfigureServices
    {
        public static void AddUserManagement(this IServiceCollection services)
        {
            services.AddDbContext<UserDbContext>();
            services.AddMediatR(typeof(ConfigureServices));
            services.AddControllers();
        }

        public static void AddUserManagement(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllers();
        }
    }
}