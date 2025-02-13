using API.Domain.Specifications;
using System;

namespace API.Domain.Users.Auth.Specifications
{
    internal class AppUserRoleShouldHaveRoleId : SpecificationBase<AppUserRole>
    {
        public override string Message => "Id da role é obrigatória.";
        public override string Code => AppUserRoleNotificationCodes.AppUserRole_01.ToString();
        public override string DetailMessage => "";

        public override Func<AppUserRole, bool> Condition() =>
            role => !string.IsNullOrEmpty(role.RoleId);
    }
}