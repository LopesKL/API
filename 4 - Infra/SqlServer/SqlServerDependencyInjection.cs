using API.Infra.SqlServer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using API.Infra.SqlServer.Context;

namespace API.Infra.SqlServer
{
    public static class SqlServerDependencyInjection
    {
        public static IServiceCollection AddSqlServerDependency(this IServiceCollection services)
        {
            services.TryAddScoped<IDbContext>(provider => provider.GetService<ApiServerContext>());

            return services;
        }
    }
}
