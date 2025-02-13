using Microsoft.Extensions.DependencyInjection.Extensions;
using API.Domain.Interfaces.Write;
using API.Infra.Repositories.Repositories;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RepositoryDependencyInjection
    {
        public static IServiceCollection AddRepositoriesDependency(this IServiceCollection services)
        {
            services.TryAddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

            return services;
        }
    }
}