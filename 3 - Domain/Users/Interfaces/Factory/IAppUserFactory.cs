using static API.Domain.Users.Auth.AppUserFactory;

namespace API.Domain.Users.Interfaces.Factory
{
    public interface IAppUserFactory
    {
        AppUserBuilder DefaultBuilder();
    }
}
