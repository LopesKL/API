
using API.Domain.Notifications;
using API.Domain.Users.Interfaces.Factory;

namespace API.Domain.Users.Auth
{
    public class AppRoleFactory : IAppRoleFactory
    {
        private readonly INotificationHandler _notificationHandler;
        public AppRoleFactory(INotificationHandler notificationHandler) =>
            _notificationHandler = notificationHandler;

        public AppRoleBuilder DefaultBuilder()
            => new AppRoleBuilder(_notificationHandler);

        public class AppRoleBuilder
        {
            private readonly AppRole _AppRole;
            private readonly INotificationHandler _notificationHandler;
            internal AppRoleBuilder(INotificationHandler notificationHandler)
            {
                _notificationHandler = notificationHandler;
                _AppRole = new AppRole(notificationHandler);
            }

            public AppRoleBuilder Id(string value)
            {
                _AppRole.Id = value;
                return this;
            }

            public AppRoleBuilder Name(string value)
            {
                _AppRole.Name = value;
                return this;
            }

            public AppRoleBuilder NormalizedName(string value)
            {
                _AppRole.NormalizedName = value;
                return this;
            }

            public AppRole Raise()
            {
                _AppRole.Specify();

                return _AppRole;
            }
        }
    }
}