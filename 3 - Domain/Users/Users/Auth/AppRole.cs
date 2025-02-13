using API.Domain.Users.Auth;
using Microsoft.AspNetCore.Identity;
using API.Domain.Notifications;
using System.Collections.Generic;
using API.Domain.Users.Auth.Specifications;

namespace API.Domain.Users.Auth
{
    public class AppRole : IdentityRole
    {
        private readonly INotificationHandler _notificationHandler;
        public AppRole() { }

        public virtual ICollection<AppUserRole> AppUserRoles { get; set; }

        internal AppRole(INotificationHandler notificationHandler)
            => _notificationHandler = notificationHandler;

        public void Specify()
        {
            var specifications = new AppRoleSpecifications();
            foreach (var specification in specifications)
            {
                var validation = specification.Condition();
                if (!validation(this))
                    _notificationHandler
                        .DefaultBuilder()
                        .Code(specification.Code)
                        .Message(specification.Message)
                        .DetailMessage(specification.DetailMessage)
                        .RaiseNotification();
            }
        }
    }
}