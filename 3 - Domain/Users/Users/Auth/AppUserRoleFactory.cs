using API.Domain.Notifications;
using API.Domain.Users.Interfaces.Factory;

namespace API.Domain.Users.Auth
{
    public class AppUserRoleFactory: IAppUserRoleFactory
    {
        private readonly INotificationHandler _notificationHandler;
        public AppUserRoleFactory(INotificationHandler notificationHandler) => 
            _notificationHandler = notificationHandler;

        public AppUserRoleBuilder DefaultBuilder()
            => new AppUserRoleBuilder(_notificationHandler);

        public class AppUserRoleBuilder
        {
            private readonly AppUserRole _appuserrole;
            private readonly INotificationHandler _notificationHandler;
            internal AppUserRoleBuilder(INotificationHandler notificationHandler)
            {
                _notificationHandler = notificationHandler;
                _appuserrole = new AppUserRole(notificationHandler);
            }
            
            public AppUserRoleBuilder RoleId(string value)
            {
                _appuserrole.RoleId = value;
                return this;
            }

            public AppUserRoleBuilder UserId(string value)
            {
                _appuserrole.UserId = value;
                return this;
            }
        }
    }
}