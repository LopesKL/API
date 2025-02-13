using static API.Domain.Users.Auth.AppRoleFactory;

namespace API.Domain.Users.Interfaces.Factory
{
    public interface IAppRoleFactory
    {
        AppRoleBuilder DefaultBuilder();
    }
}
