using API.Application.Dto;
using API.Application.Dto.Response;
using API.Application.Users;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using INotification = API.Domain.Notifications.INotificationHandler;

namespace API.WebApi.Controllers.Base
{
    [ApiController]
    public class BaseController(UserHandler handler, INotification notification) : ControllerBase
    {
        private UserDto _currentUser;

        public UserDto CurrentUser => GetCurrentUser();

        private UserDto GetCurrentUser()
        {
            if (!User.Identity.IsAuthenticated)
                return null;

            if (_currentUser != null)
                return _currentUser;

            var userByIdentity = handler.GetUserByLogin(User.Identity.Name);
            userByIdentity.Wait();
            _currentUser = userByIdentity.Result;

            return _currentUser;
        }

        // Método auxiliar para verificar notificações e retornar resposta apropriada
        protected ActionResult HandleResponse<T>(T result)
        {
            if (notification.HasNotification())
                return BadRequest(new BadRequestDto(notification));

            return Ok(new OkDto<T>(result));
        }

        // Novo método para lidar com respostas de arquivos
        protected ActionResult HandleFileResponse(Stream fileStream, string contentType, string fileName)
        {
            if (notification.HasNotification())
                return BadRequest(new BadRequestDto(notification));

            return File(fileStream, contentType, fileName);
        }

        // Método genérico para GET
        protected async Task<ActionResult> Get<TRequest, TResponse>(TRequest request, Func<TRequest, Task<TResponse>> handlerMethod)
        {
            var result = await handlerMethod(request);
            return HandleResponse(result);
        }

        // Método genérico para GET de arquivos usando uma tupla
        protected async Task<ActionResult> GetFile<TRequest>(TRequest request, Func<TRequest, Task<(Stream, string)>> handlerMethod)
        {
            var (fileStream, contentType) = await handlerMethod(request);
            return HandleFileResponse(fileStream, contentType, request.ToString()); // O request pode ser usado como fileName, ou modifique conforme necessário
        }

        // Método genérico para POST
        protected async Task<ActionResult> Post<TRequest, TResponse>(TRequest request, Func<TRequest, Task<TResponse>> handlerMethod) where TRequest : class
        {
            var result = await handlerMethod(request);
            return HandleResponse(result);
        }

        // Método genérico para DELETE
        protected async Task<ActionResult> Delete<TResponse>(Guid id, Func<Guid, Task<TResponse>> handlerMethod)
        {
            var result = await handlerMethod(id);
            return HandleResponse(result);
        }
    }
}
