using API.Domain.Notifications;
using API.Domain.Users.Interfaces.Factory;

namespace API.Domain.Users.Auth
{
    public class AppUserFactory : IAppUserFactory
    {
        private readonly INotificationHandler _notificationHandler;
        public AppUserFactory(INotificationHandler notificationHandler) =>
            _notificationHandler = notificationHandler;

        public AppUserBuilder DefaultBuilder()
            => new AppUserBuilder(_notificationHandler);

        public class AppUserBuilder
        {
            private readonly AppUser _appuser;
            private readonly INotificationHandler _notificationHandler;
            internal AppUserBuilder(INotificationHandler notificationHandler)
            {
                _notificationHandler = notificationHandler;
                _appuser = new AppUser(notificationHandler);
            }

            public AppUserBuilder UserName(string value)
            {
                _appuser.UserName = value;
                return this;
            }

            public AppUserBuilder Email(string value)
            {
                _appuser.Email = value;
                return this;
            }

            public AppUserBuilder Active(bool value)
            {
                _appuser.Active = value;
                return this;
            }

            public AppUser Raise()
            {
                _appuser.Specify();
                if (_notificationHandler.HasNotification())
                    return new AppUser(_notificationHandler);

                return _appuser;
            }
        }
    }
}