//using API.Application.Dto;
//using API.Application.Dto.Request;
//using API.Application.Users;
//using API.Domain.Users.Auth.JWT;
//using API.WebApi.Controllers.Base;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Threading.Tasks;
//using INotification = API.Domain.Notifications.INotificationHandler;

//namespace API.WebApi.Controllers {
//    [Authorize(Roles = Roles.ROLE_ADMIN)]  // Restringe o acesso ao controller apenas para usuários com a role ADMIN
//    public class UsuariosController(UserHandler userHandler, UsuariosHandler handler, INotification notification) : BaseController(userHandler, notification) {
//        // Endpoint para buscar todos os registros com base em um RequestAllDto via POST
//        [HttpPost("/crud/getAll")]
//        public Task<ActionResult> GetAll(RequestAllUserDto request) => Post(request, handler.GetAll);

//        // Endpoint para buscar um registro com base em um ID via GET
//        [HttpGet("/crud/getById/{id}")]
//        public Task<ActionResult> GetById(Guid id) => Get(id, handler.GetById);

//        // Endpoint para inserir ou atualizar um registro, recebe um objeto CrudDto via POST
//        [HttpPost("/crud/upsert")]
//        public Task<ActionResult> Upsert(UserDto crud) => Post(crud, r => handler.Upsert(r, CurrentUser));

//        // Endpoint para deletar um registro com base no ID fornecido via DELETE
//        [HttpDelete("/crud/{id}")]
//        public Task<ActionResult> Delete(Guid id) => Delete(id, r => handler.Delete(r, CurrentUser));

//    }
//}