using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasyIdentity.Api
{
    public static class ConfigurationExtensions
    {
        public static void EnsureMigrationOfContext<T>(this IApplicationBuilder app) where T : DbContext
        {
            var dbContext = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope().ServiceProvider.GetService<T>();
            if(dbContext is null)
            {
                throw new System.Exception($"No DbContext of type '{typeof(T)}' registered in Services");
            }
            dbContext.Database.Migrate();
        }
    }
}