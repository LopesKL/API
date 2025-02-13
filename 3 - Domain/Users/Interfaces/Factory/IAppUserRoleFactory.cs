using static API.Domain.Users.Auth.AppUserRoleFactory;

namespace API.Domain.Users.Interfaces.Factory
{
    public interface IAppUserRoleFactory
    {
        AppUserRoleBuilder DefaultBuilder();
    }
}
