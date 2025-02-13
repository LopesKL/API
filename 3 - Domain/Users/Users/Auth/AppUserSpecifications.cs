using API.Domain.Specifications;
using System.Collections.Generic;

namespace API.Domain.Users.Auth.Specifications
{
    internal class AppUserSpecifications : List<SpecificationBase<AppUser>>
    {
        public AppUserSpecifications()
        {
            AddRange(new List<SpecificationBase<AppUser>>() {
                new AppUserShouldHaveUserName(),
                new AppUserShouldHaveEmail()
            });
        }
    }
}