using API.Application.Dto;
using API.Application.Users;
using API.Domain.Users.Auth.JWT;
using API.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using INotification = API.Domain.Notifications.INotificationHandler;

namespace API.WebApi.Controllers
{
    [Authorize]
    public class SignInController(UserHandler handler, INotification notification) : BaseController(handler, notification)
    {
        [AllowAnonymous]
        [HttpPost("/signin")]
        public Task<ActionResult> Signin(UserSignInDto request) => Post(request, handler.SignIn);

        [Authorize(Roles = Roles.ROLE_ADMIN)]
        [HttpPost("/seed")]
        public Task<ActionResult> Seed() => Post<object, bool>(null, _ => handler.SeedAdmin());
    }
}
