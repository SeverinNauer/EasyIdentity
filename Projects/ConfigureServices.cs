using Microsoft.Extensions.DependencyInjection;
using Projects.Contracts;
using System;

namespace Projects
{
    public static class ConfigureServices
    {
        public static void AddProjects(this IServiceCollection services)
        {
            services.AddTransient<IQueryProjectId, QueryProjectId>();
        }
    }
}
