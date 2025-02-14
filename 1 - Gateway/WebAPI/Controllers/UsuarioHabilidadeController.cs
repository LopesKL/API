using API.Application.Dto;
using API.Application.Dto.Request;
using API.Application.Users;
using API.Domain.Users.Auth.JWT;
using API.WebApi.Controllers.Base;
using Application.UsuarioHabilidadeHandler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using INotification = API.Domain.Notifications.INotificationHandler;

namespace API.WebApi.Controllers {
   // [Authorize(Roles = Roles.ROLE_ADMIN)]  // Restringe o acesso ao controller apenas para usuários com a role ADMIN
    public class UsuarioHabilidadeController(UserHandler userHandler, UsuarioHabilidadeHandler handler, INotification notification) : BaseController(userHandler, notification) {
        // Endpoint para buscar todos os registros com base em um RequestAllDto via POST
        [HttpPost("/usuarioHabilidade/getAll")]
        public Task<ActionResult> GetAll(RequestAllUsuarioHabilidadeDto request) => Post(request, handler.GetAll);

        // Endpoint para buscar um registro com base em um ID via GET
        //[HttpGet("/usuarioHabilidade/getById/{id}")]
        //public Task<ActionResult> GetById(Guid id) => Get(id, handler.GetById);

        // Endpoint para inserir ou atualizar um registro, recebe um objeto CrudDto via POST
        [HttpPost("/usuarioHabilidade/upsert")]
        public Task<ActionResult> Upsert(UsuarioHabilidadeDto crud) => Post(crud, r => handler.Upsert(r, CurrentUser));

        //Arrumer metodo Delete
        //[HttpDelete("/usuarioHabilidade/{id}")]
        //public Task<ActionResult> Delete(Guid id) => Delete(id, r => handler.Delete(r, CurrentUser));

    }
}
