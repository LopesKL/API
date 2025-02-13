using API.Domain.Notifications;
using API.Domain.Users.Auth.Specifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;
using System;

namespace API.Domain.Users.Auth
{
    public class AppUser : IdentityUser
    {
        private readonly INotificationHandler _notificationHandler;

        public AppUser() { }
        private AppUser(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        private ILazyLoader LazyLoader { get; set; }
        internal AppUser(INotificationHandler notificationHandler)
        {
            _notificationHandler = notificationHandler;
        }

        public int TaxaFaturamento { get; set; }
        public string Empresa { get; set; }
        public bool Active { get; set; }
        public DateTime Updated { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastLogin { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }

        public virtual ICollection<AppUserRole> AppUserRoles { get; set; }

        public void Specify()
        {
            var specifications = new AppUserSpecifications();
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