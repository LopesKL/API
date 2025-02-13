using System.Globalization;

namespace API.Domain.Users.Auth.JWT
{
    public static class Roles
    {
        public const string ROLE_ADMIN = "RoleAdmin";
        public const string ROLE_USER = "RoleUser";
        public const string ROLE_PROJECTMANAGER = "RoleProjectManager";

        public static (string Name, string Id) ROLE_ADMIN_CREATE = (ROLE_ADMIN, "23d9d409-d7aa-4966-9047-48c04b41f0a1".ToUpper());
        public static (string Name, string Id) ROLE_PROJECTMANAGER_CREATE = (ROLE_PROJECTMANAGER, "373c7921-df59-4d99-9827-521f7fbc97b6".ToUpper());
        public static (string Name, string Id) ROLE_USER_CREATE = (ROLE_USER, "b47591c2-1033-4543-b400-83b83c63b1bd".ToUpper());



    }
}
