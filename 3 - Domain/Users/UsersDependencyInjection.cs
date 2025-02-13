using API.Domain.Users.Auth;
using API.Domain.Users.Interfaces.Factory;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class UsersDependencyInjection
    {
        public static IServiceCollection AddUsersDependency(this IServiceCollection services)
        {
            services.TryAddTransient<IAppUserFactory, AppUserFactory>();
            services.TryAddTransient<IAppRoleFactory, AppRoleFactory>();

            return services;
        }
    }
}
