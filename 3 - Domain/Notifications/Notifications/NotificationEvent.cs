using System;

namespace API.Domain.Notifications
{
    public class NotificationEvent
    {
        internal NotificationEvent() => Identifier = Guid.NewGuid();
        public Guid Identifier { get; }
        public string Code { get; set; }
        public string Message { get; set; }
        public string DetailMessage { get; set; }
    }
}
