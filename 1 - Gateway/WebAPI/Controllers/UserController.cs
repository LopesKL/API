 using API.Application.Dto;
using API.Application.Dto.Request;
using API.Application.Users;
using API.Domain.Users.Auth.JWT;
using API.WebApi.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using static API.Application.Dto.UserDto;
using INotification = API.Domain.Notifications.INotificationHandler;

namespace WebAPI.Controllers.Usuarios
{
    [Authorize(Roles = $"{Roles.ROLE_ADMIN_EMPRESA},{Roles.ROLE_ATENDENTE},{Roles.ROLE_ADMIN}")]

    public class UserController(SignInHandler signInHandler, UserHandler userHandler, INotification notification) : BaseController(signInHandler, notification)
    {
        // Endpoint para buscar todos os registros com base em um RequestAllDto via POST
        [HttpPost("/user/getAll")]
        public Task<ActionResult> GetAll(RequestAllUserDto request) => Post(request, r => userHandler.GetAll(r, CurrentUser));
        
        //Endpoint para buscar todos os motoristas
        [HttpPost("/user/getAllMotorista")]
        public Task<ActionResult> GetAllMotorista(RequestAllUserDto request) => Post(request, userHandler.GetAllMotorista);
        // Endpoint para buscar um registro com base em um ID via GET
        [HttpGet("/user/getById/{id}")]
        public Task<ActionResult> GetById(string id) => Get(id, userHandler.GetById);

        // Endpoint para inserir ou atualizar um registro, recebe um objeto CrudDto via POST
        [HttpPost("/user/upsert")]
        public Task<ActionResult> Upsert(UserDto crud) => Post(crud, r => userHandler.Upsert(r, CurrentUser));


        // Endpoint para deletar um registro com base no ID fornecido via DELETE
        [HttpDelete("/user/{id}")]
        public Task<ActionResult> Delete(Guid id) => Delete(id, r => userHandler.Delete(r, CurrentUser));
    }

}
