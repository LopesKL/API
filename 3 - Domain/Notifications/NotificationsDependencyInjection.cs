using Microsoft.Extensions.DependencyInjection.Extensions;
using API.Domain.Notifications;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class NotificationsDependencyInjection
    {
        public static IServiceCollection AddNotificationsDependency(this IServiceCollection services)
        {
            services.TryAddScoped<INotificationHandler, NotificationHandler>();
            return services;
        }
    }
}
