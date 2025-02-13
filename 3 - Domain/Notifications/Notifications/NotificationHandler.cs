using System.Collections.Generic;
using System.Linq;

namespace API.Domain.Notifications
{
    public class NotificationHandler : INotificationHandler
    {
        private readonly List<NotificationEvent> _notifications;
        public NotificationHandler()
        {
            _notifications = new List<NotificationEvent>();
        }

        public void ClearNotifications()
        {
            _notifications.Clear();
        }

        public NotificationBuilder DefaultBuilder() => new NotificationBuilder(_notifications);

        public List<NotificationEvent> GetAllNotifications() => _notifications;

        public bool HasNotification() => _notifications.Any();

        public class NotificationBuilder
        {
            private readonly List<NotificationEvent> _notifications;
            private readonly NotificationEvent _notification;
            public NotificationBuilder(List<NotificationEvent> notifications)
            {
                _notifications = notifications;
                _notification = new NotificationEvent();
            }


            public NotificationBuilder Code(string code)
            {
                _notification.Code = code;
                return this;
            }

            public NotificationBuilder Message(string message)
            {
                _notification.Message = message;
                return this;
            }
            public NotificationBuilder DetailMessage(string detailMessage)
            {
                _notification.DetailMessage = detailMessage;
                return this;
            }

            public void RaiseNotification() => _notifications.Add(_notification);
        }
    }
}
