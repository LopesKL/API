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

namespace Application.AtividadePaiHandler {
    public class AtividadePaiHandler {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly INotification _notification;

        public AtividadePaiHandler(IUnitOfWork uow, IMapper mapper, INotification notification, IConfiguration configuration) {
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

        public async Task<ResponseAllDto<List<AtividadePaiDto>>> GetAll(RequestAllAtividadePaiDto request) {
            var consultaBase = _uow.AtividadePaiRepository.Find(x => !x.IsDeleted).AsQueryable();
            consultaBase = consultaBase.ApplyFilters(request);

            if (!string.IsNullOrEmpty(request.SortOrder) && !string.IsNullOrEmpty(request.SorterField))
                consultaBase = consultaBase.ApplySorting(request.SorterField, request.SortOrder);
            else
                consultaBase = consultaBase.OrderByDescending(x => x.Updated).ThenByDescending(x => x.Created);

            var totalItens = await consultaBase.CountAsync();
            var itensPaginados = await consultaBase
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => _mapper.Map<AtividadePaiDto>(x))
                .ToListAsync();

            return new ResponseAllDto<List<AtividadePaiDto>>(itensPaginados, totalItens, itensPaginados.Count);
        }

        public async Task<AtividadePaiDto> GetById(Guid id) {
            var atividadePai = _uow.AtividadePaiRepository.Find(x => !x.IsDeleted && x.IdAtividadePai == id).FirstOrDefault();
            return _mapper.Map<AtividadePaiDto>(atividadePai);
        }

        public async Task<AtividadePaiDto> Upsert(AtividadePaiDto atividadePaiDto, UserDto currentUser) {
            try {
                var atividadePai = _uow.AtividadePaiRepository.Find(p => p.IdAtividadePai == atividadePaiDto.IdAtividadePai).FirstOrDefault();
                bool insert = atividadePai == null;

                if (insert) {
                    atividadePai = new AtividadePai { IdAtividadePai = Guid.NewGuid() };
                }

                atividadePai.IdProjeto = atividadePaiDto.IdProjeto;
                atividadePai.Nome = atividadePaiDto.Nome;
                atividadePai.DataInicio = atividadePaiDto.DataInicio;
                atividadePai.DataFim = atividadePaiDto.DataFim;
                atividadePai.HorasEstimadas = atividadePaiDto.HorasEstimadas;
                atividadePai.HorasCobradas = atividadePaiDto.HorasCobradas;
                atividadePai.HorasNaoCobradas = atividadePaiDto.HorasNaoCobradas;
                atividadePai.Descricao = atividadePaiDto.Descricao;
                atividadePai.Files = atividadePaiDto.Files;

                if (insert) {
                    atividadePai.Created = DateTime.UtcNow;
                    atividadePai.CreatedBy = new Guid(currentUser.Id);
                    atividadePai.IsDeleted = false;
                    _uow.AtividadePaiRepository.Insert(atividadePai);
                }
                else {
                    atividadePai.Updated = DateTime.UtcNow;
                    atividadePai.UpdatedBy = new Guid(currentUser.Id);
                    _uow.AtividadePaiRepository.Update(atividadePai);
                }

                await _uow.Save();
                return _mapper.Map<AtividadePaiDto>(atividadePai);
            }
            catch {
                return null;
            }
        }

        public async Task<bool> Delete(Guid id, UserDto currentUser) {
            var atividadePai = _uow.AtividadePaiRepository.Find(x => x.IdAtividadePai == id).FirstOrDefault();
            if (atividadePai != null) {
                atividadePai.Updated = DateTime.UtcNow;
                atividadePai.UpdatedBy = new Guid(currentUser.Id);
                atividadePai.IsDeleted = true;
                _uow.AtividadePaiRepository.Update(atividadePai);
                await _uow.Save();
                return true;
            }
            return false;
        }
    }
}
