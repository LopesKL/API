using API.Application.Dto;
using API.Application.Dto.Request;
using API.Application.Users;
using API.Domain.Users.Auth.JWT;
using API.WebApi.Controllers.Base;
using Application.TagHandler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using INotification = API.Domain.Notifications.INotificationHandler;

namespace API.WebApi.Controllers {
    //[Authorize(Roles = Roles.ROLE_ADMIN)]  // Restringe o acesso ao controller apenas para usuários com a role ADMIN
    public class TagController(UserHandler userHandler, TagHandler handler, INotification notification) : BaseController(userHandler, notification) {
        // Endpoint para buscar todos os registros com base em um RequestAllDto via POST
        [HttpPost("/tag/getAll")]
        public Task<ActionResult> GetAll(RequestAllTagDto request) => Post(request, handler.GetAll);

        // Endpoint para buscar um registro com base em um ID via GET
        [HttpGet("/tag/getById/{id}")]
        public Task<ActionResult> GetById(Guid id) => Get(id, handler.GetById);

        // Endpoint para inserir ou atualizar um registro, recebe um objeto CrudDto via POST
        [HttpPost("/tag/upsert")]
        public Task<ActionResult> Upsert(TagDto crud) => Post(crud, r => handler.Upsert(r, CurrentUser));

        // Endpoint para deletar um registro com base no ID fornecido via DELETE
        [HttpDelete("/tag/{id}")]
        public Task<ActionResult> Delete(Guid id) => Delete(id, r => handler.Delete(r, CurrentUser));
    }
}
