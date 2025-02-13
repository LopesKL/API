using static API.Domain.Notifications.NotificationHandler;
using System.Collections.Generic;

namespace API.Domain.Notifications
{
    public interface INotificationHandler
    {
        NotificationBuilder DefaultBuilder();
        List<NotificationEvent> GetAllNotifications();
        void ClearNotifications();
        bool HasNotification();
    }
}
