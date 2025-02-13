using API.Domain.Notifications;
using Microsoft.AspNetCore.Identity;

namespace API.Domain.Users.Auth
{
    public class AppUserRole : IdentityUserRole<string> 
    {
        private readonly INotificationHandler _notificationHandler;

        internal AppUserRole() { }

        public virtual AppUser AppUser { get; set; }
        public virtual AppRole AppRole{ get; set; }

        internal AppUserRole(INotificationHandler notificationHandler)
           => _notificationHandler = notificationHandler;
    }
}
