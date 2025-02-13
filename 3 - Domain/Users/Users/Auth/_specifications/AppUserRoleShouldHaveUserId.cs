using API.Domain.Specifications;
using System;

namespace API.Domain.Users.Auth.Specifications
{
    internal class AppUserRoleShouldHaveUserId : SpecificationBase<AppUserRole>
    {
        public override string Message => "Id da role é obrigatória.";
        public override string Code => AppUserRoleNotificationCodes.AppUserRole_02.ToString();
        public override string DetailMessage => "";

        public override Func<AppUserRole, bool> Condition() =>
            role => !string.IsNullOrEmpty(role.UserId);
    }
}