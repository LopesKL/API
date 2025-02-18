using API.Application.Dto;
using API.Application.Dto.Request;
using API.Application.Users;
using API.Domain.Users.Auth.JWT;
using API.WebApi.Controllers.Base;
using Application.AtividadeUsuarioHandler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using INotification = API.Domain.Notifications.INotificationHandler;

namespace API.WebApi.Controllers {
    //[Authorize(Roles = Roles.ROLE_ADMIN)]  // Restringe o acesso ao controller apenas para usuários com a role ADMIN
    public class AtividadeFilhoUsuarioController(UserHandler userHandler, AtividadeUsuarioHandler handler, INotification notification) : BaseController(userHandler, notification) {
        [HttpPost("/atividadeUsuario/getAll")]
        public Task<ActionResult> GetAll(RequestAllAtividadeUsuarioDto request) => Post(request, handler.GetAll);

        //[HttpGet("/atividadeFilhoUsuario/getById/{id}")]
        //public Task<ActionResult> GetById(Guid id) => Get(id, handler.GetById);

        [HttpGet("/atividadeUsuario/getInicio")]
        public Task<ActionResult> GetInicio() => Get(CurrentUser, handler.GetUserActivitiesTeste);


        [HttpGet("/atividadeUsuario/getTimeSheet")]
        public async Task<ActionResult> GetTimeSheet(DateTime startDate, DateTime endDate)
        {
            var currentUser = CurrentUser; // Assumindo que você tem um método para obter o usuário atual.
            var result = await handler.GetUserTimeSheet(currentUser, startDate, endDate);

            return Ok(result);
        }



        [HttpPost("/atividadeUsuario/upsert")]
        public Task<ActionResult> Upsert(AtividadeUsuarioDto crud) => Post(crud, r => handler.Upsert(r, CurrentUser));

        //Arrumar metodo Delete
        //[HttpDelete("/atividadeFilhoUsuario/{idUsuario}/{idAtividadeFilho}")]
        //public Task<ActionResult> Delete(Guid idUsuario, Guid idAtividadeFilho) => handler.Delete(idUsuario, idAtividadeFilho);

    }
}
