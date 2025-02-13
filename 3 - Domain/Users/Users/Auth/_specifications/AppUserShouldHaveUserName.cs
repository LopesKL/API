using API.Domain.Specifications;
using System;

namespace API.Domain.Users.Auth.Specifications
{
    internal class AppUserShouldHaveUserName : SpecificationBase<AppUser>
    {
        public override string Message => "Login do usuário é obrigatório.";
        public override string Code => AppUserNotificationCodes.AppUser_02.ToString();
        public override string DetailMessage => "";

        public override Func<AppUser, bool> Condition() =>
            idea => !string.IsNullOrEmpty(idea.UserName);
    }
}