using API.Application.Dto;
using API.Application.Dto.Request;
using API.Application.Users;
using API.Domain.Users.Auth.JWT;
using API.WebApi.Controllers.Base;
using Application.PropjetosHandler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using INotification = API.Domain.Notifications.INotificationHandler;

namespace API.WebApi.Controllers {
    [Authorize(Roles = Roles.ROLE_ADMIN)]  // Restringe o acesso ao controller apenas para usuários com a role ADMIN
    public class ProjetosController(UserHandler userHandler, ProjetosHandler handler, INotification notification) : BaseController(userHandler, notification) {
        // Endpoint para buscar todos os registros com base em um RequestAllDto via POST
        [HttpPost("/projetos/getAll")]
        public Task<ActionResult> GetAll(RequestAllProjetosDto request) => Post(request, handler.GetAll);

        // Endpoint para buscar um registro com base em um ID via GET
        [HttpGet("/projetos/getById/{id}")]
        public Task<ActionResult> GetById(Guid id) => Get(id, handler.GetById);

        // Endpoint para inserir ou atualizar um registro, recebe um objeto CrudDto via POST
        [HttpPost("/projetos/upsert")]
        public Task<ActionResult> Upsert(ProjetosDto crud) => Post(crud, r => handler.Upsert(r, CurrentUser));

        // Endpoint para deletar um registro com base no ID fornecido via DELETE
        [HttpDelete("/projetos/{id}")]
        public Task<ActionResult> Delete(Guid id) => Delete(id, r => handler.Delete(r, CurrentUser));

        // Endpoint para fazer upload de um arquivo, recebendo os dados do arquivo via POST usando o atributo FromForm
        [HttpPost("/projetos/uploadFile")]
        public Task<ActionResult> UploadFile([FromForm] FileUploadRequest request) => Post(request, handler.UploadFile);

        // Endpoint para remover um arquivo, recebendo o nome do arquivo via parâmetro da URL
        [HttpGet("/projetos/getFile/{file}")]
        public Task<ActionResult> GetFile(string file) => GetFile(file, handler.GetFile);

        // Endpoint para remover um arquivo, recebendo o nome do arquivo via parâmetro da URL
        [HttpPost("/projetos/removeFile/{file}")]
        public Task<ActionResult> RemoveFile(string file) => Post(file, handler.RemoveFile);
    }
}
