using API.Application.Dto;
using API.Application.Dto.Request;
using API.Application.Users;
using API.Domain.Users.Auth.JWT;
using API.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static API.Application.Dto.UserDto;
using INotification = API.Domain.Notifications.INotificationHandler;

namespace WebAPI.Controllers.Usuarios
{

    public class UserController(UserHandler signInHandler, UsuariosHandler userHandler, INotification notification) : BaseController(signInHandler, notification)
    {
        // Endpoint para buscar todos os registros com base em um RequestAllDto via POST
        [HttpPost("/user/getAll")]
        public Task<ActionResult> GetAll(RequestAllUserDto request) => Post(request,r => userHandler.GetAll(r, CurrentUser));

        // Endpoint para buscar um registro com base em um ID via GET
        [HttpGet("/user/getById/{id}")]
        public Task<ActionResult> GetById(string id) => Get(id, userHandler.GetById);

        // Endpoint para inserir ou atualizar um registro, recebe um objeto CrudDto via POST
        [HttpPost("/user/upsert")]
        public Task<ActionResult> Upsert(UserDto crud) => Post(crud, r => userHandler.Upsert(r, CurrentUser));


        // Endpoint para deletar um registro com base no ID fornecido via DELETE
        [HttpDelete("/user/{id}")]
        public Task<ActionResult> Delete(Guid id) => Delete(id, r => userHandler.Delete(r, CurrentUser));


        [HttpGet("/user/getRoles")]
        public async Task<ActionResult<List<RoleDto>>> GetRoles()
        {
            var roles = await userHandler.GetRoles();
            return Ok(roles);
        }
    }

}
