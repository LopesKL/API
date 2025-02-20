using API.Application.Blobs;
using API.Application.Dto;
using API.Application.Dto.Request;
using API.Application.Dto.ResponsePatterns;
using API.Domain.Interfaces.Write;
using API.Domain.Projeto;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using INotification = API.Domain.Notifications.INotificationHandler;

namespace Application.ComentarioHandler {
    public class ComentarioHandler {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly INotification _notification;

        public ComentarioHandler(IUnitOfWork uow, IMapper mapper, INotification notification, IConfiguration configuration) {
            _uow = uow;
            _mapper = mapper;
            _notification = notification;
            _configuration = configuration;
        }
        public async Task<string> UploadFile(FileUploadRequest request) {
            try {
                // Instancia um objeto de BlobStorage e busca o arquivo pelo identificador (key)
                var blobStorage = new BlobStorage(_configuration);
                var file = request.Files.FirstOrDefault();
                var fileExtension = Path.GetExtension(file.FileName);
                var newFilename = Guid.NewGuid().ToString() + fileExtension;

                await blobStorage.UploadFile(file.OpenReadStream(), newFilename, file.ContentType);
                return newFilename; // Retorna o nome do blob salvo
            }
            catch {
                return null; // Se ocorrer um erro, retorna null
            }
        }

        // Método para buscar um arquivo no Azure Blob Storage
        public async Task<(Stream, string)> GetFile(string key) {
            try {
                // Instancia um objeto de BlobStorage e busca o arquivo pelo identificador (key)
                var blobStorage = new BlobStorage(_configuration);
                var (fileStream, contentType) = await blobStorage.GetFile(key);

                return (fileStream, contentType); // Retorna o stream do arquivo e o tipo de conteúdo
            }
            catch {
                return (null, null); // Se ocorrer um erro, retorna null
            }
        }

        public async Task<bool> RemoveFile(string filename) {
            try {
                // Instancia um objeto de BlobStorage e busca o arquivo pelo identificador (key)
                var blobStorage = new BlobStorage(_configuration);
                await blobStorage.RemoveFile(filename);
                return true;
            }
            catch {
                return false;
            }
        }

        public async Task<ResponseAllDto<List<ComentarioDto>>> GetAll(RequestAllComentarioDto request) {
            var consultaBase = _uow.ComentarioRepository.Find(x => !x.IsDeleted).AsQueryable();

            // Aplicação dos filtros considerando que os IDs são Guid
            if (request.IdAtividade != null) {
                // Filtro pelo idAtividade (nível mais específico)
                consultaBase = consultaBase.Where(x => x.IdAtividade == request.IdAtividade);
            }
            else if (request.IdAtividadeFilho != null) {
                // Se idAtividade não estiver preenchido, utiliza idAtividadeFilho
                consultaBase = consultaBase.Where(x => x.IdAtividadeFilho == request.IdAtividadeFilho);
            }
            else if (request.IdAtividadePai != null) {
                // Caso os anteriores não estejam preenchidos, aplica o filtro de idAtividadePai
                consultaBase = consultaBase.Where(x => x.IdAtividadePai == request.IdAtividadePai);
            }
            else if (request.IdProjetos != null) {
                // Por último, filtra pelo idProjetos
                consultaBase = consultaBase.Where(x => x.IdProjetos == request.IdProjetos);
            }

            // Ordenação
            if (!string.IsNullOrEmpty(request.SortOrder) && !string.IsNullOrEmpty(request.SorterField)) {
                consultaBase = consultaBase.ApplySorting(request.SorterField, request.SortOrder);
            }
            else {
                consultaBase = consultaBase.OrderByDescending(x => x.Updated)
                                           .ThenByDescending(x => x.Created);
            }

            // Paginação e mapeamento para DTO
            var totalItens = await consultaBase.CountAsync();
            var itensPaginados = await consultaBase
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => _mapper.Map<ComentarioDto>(x))
                .ToListAsync();

            return new ResponseAllDto<List<ComentarioDto>>(itensPaginados, totalItens, itensPaginados.Count);
        }

        public async Task<ComentarioDto> GetById(Guid id) {
            var comentario = _uow.ComentarioRepository.Find(x => !x.IsDeleted && x.IdComentario == id).FirstOrDefault();
            return _mapper.Map<ComentarioDto>(comentario);
        }

        public async Task<ComentarioDto> Upsert(ComentarioDto comentarioDto, UserDto currentUser) {
            try {
                var comentario = _uow.ComentarioRepository.Find(p => p.IdComentario == comentarioDto.IdComentario).FirstOrDefault();
                bool insert = comentario == null;

                if (insert) {
                    comentario = new Comentario { IdComentario = Guid.NewGuid() };
                }

                comentario.IdUsuario = comentarioDto.IdUsuario;
                comentario.IdProjetos = comentarioDto.IdProjetos;
                comentario.IdAtividadePai = comentarioDto.IdAtividadePai;
                comentario.IdAtividadeFilho = comentarioDto.IdAtividadeFilho;
                comentario.IdLancamento = comentarioDto.IdLancamento;
                comentario.IdAtividade = comentarioDto.IdAtividade;
                comentario.Texto = comentarioDto.Texto;
                comentario.Files = comentarioDto.Files;

                if (insert) {
                    comentario.Created = DateTime.UtcNow;
                    //comentario.CreatedBy = new Guid(currentUser.Id);
                    comentario.IsDeleted = false;
                    _uow.ComentarioRepository.Insert(comentario);
                }
                else {
                    comentario.Updated = DateTime.UtcNow;
                    comentario.UpdatedBy = new Guid(currentUser.Id);
                    _uow.ComentarioRepository.Update(comentario);
                }

                await _uow.Save();
                return _mapper.Map<ComentarioDto>(comentario);
            }
            catch {
                return null;
            }
        }

        public async Task<bool> Delete(Guid id, UserDto currentUser) {
            var comentario = _uow.ComentarioRepository.Find(x => x.IdComentario == id).FirstOrDefault();
            if (comentario != null) {
                comentario.Updated = DateTime.UtcNow;
                comentario.UpdatedBy = new Guid(currentUser.Id);
                comentario.IsDeleted = true;
                _uow.ComentarioRepository.Update(comentario);
                await _uow.Save();
                return true;
            }
            return false;
        }
    }
}
