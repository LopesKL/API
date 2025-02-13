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
//    public class CrudController(UserHandler userHandler, CrudHandler handler, INotification notification) : BaseController(userHandler, notification) {
//        // Endpoint para buscar todos os registros com base em um RequestAllDto via POST
//        [HttpPost("/crud/getAll")]
//        public Task<ActionResult> GetAll(RequestAllCrudDto request) => Post(request, handler.GetAll);

//        // Endpoint para buscar um registro com base em um ID via GET
//        [HttpGet("/crud/getById/{id}")]
//        public Task<ActionResult> GetById(Guid id) => Get(id, handler.GetById);

//        // Endpoint para inserir ou atualizar um registro, recebe um objeto CrudDto via POST
//        [HttpPost("/crud/upsert")]
//        public Task<ActionResult> Upsert(CrudDto crud) => Post(crud, r => handler.Upsert(r, CurrentUser));

//        // Endpoint para deletar um registro com base no ID fornecido via DELETE
//        [HttpDelete("/crud/{id}")]
//        public Task<ActionResult> Delete(Guid id) => Delete(id, r => handler.Delete(r, CurrentUser));

//        // Endpoint para fazer upload de um arquivo, recebendo os dados do arquivo via POST usando o atributo FromForm
//        [HttpPost("/crud/uploadFile")]
//        public Task<ActionResult> UploadFile([FromForm] FileUploadRequest request) => Post(request, handler.UploadFile);

//        // Endpoint para remover um arquivo, recebendo o nome do arquivo via parâmetro da URL
//        [HttpGet("/crud/getFile/{file}")]
//        public Task<ActionResult> GetFile(string file) => GetFile(file, handler.GetFile);

//        // Endpoint para remover um arquivo, recebendo o nome do arquivo via parâmetro da URL
//        [HttpPost("/crud/removeFile/{file}")]
//        public Task<ActionResult> RemoveFile(string file) => Post(file, handler.RemoveFile);
//    }
//}
