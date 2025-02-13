using API.Domain.Specifications;
using System.Collections.Generic;

namespace API.Domain.Users.Auth.Specifications
{
    internal class AppUserRoleSpecifications : List<SpecificationBase<AppUserRole>>
    {
        public AppUserRoleSpecifications()
        {
            AddRange(new List<SpecificationBase<AppUserRole>>() {
                new AppUserRoleShouldHaveRoleId(),
                new AppUserRoleShouldHaveUserId()
            });
        }
    }
}