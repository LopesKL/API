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

namespace Application.AtividadeHandler {
    public class AtividadeHandler {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly INotification _notification;
        private readonly string _getAzureStorageConnectionString;
        private readonly string _getAzureBlobStorageContainer;

        public AtividadeHandler(IUnitOfWork uow, IMapper mapper, INotification notification, IConfiguration configuration) {
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

        // Método para obter uma lista paginada de atividades
        public async Task<ResponseAllDto<List<AtividadeDto>>> GetAll(RequestAllAtividadeDto request) {
            var consultaBase = _uow.AtividadeRepository.Find(x => !x.IsDeleted && x.IdAtividadeFilho == request.IdAtividadeFilho).AsQueryable();

            // Aplica os filtros dinâmicos
            //consultaBase = consultaBase.ApplyFilters(request);

            // Ordenação dinâmica
            if (!string.IsNullOrEmpty(request.SortOrder) && !string.IsNullOrEmpty(request.SorterField))
                consultaBase = consultaBase.ApplySorting(request.SorterField, request.SortOrder);
            else
                consultaBase = consultaBase.OrderByDescending(x => x.Updated).ThenByDescending(x => x.Created);

            var totalItens = await consultaBase.CountAsync();

            var itensPaginados = await consultaBase
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new AtividadeDto {
                    IdAtividade = x.IdAtividade,
                    IdAtividadeFilho = x.IdAtividadeFilho,
                    Nome = x.Nome,
                    DataInicio = x.DataInicio,
                    DataFim = x.DataFim,
                    Status = x.Status,
                    Progresso = x.Progresso,
                    HorasEstimadas = x.HorasEstimadas,
                    HorasTotais = x.HorasTotais,
                    Cobrado = x.Cobrado,
                    Created = x.Created,
                    Updated = x.Updated,
                    CreatedBy = x.CreatedBy,
                    UpdatedBy = x.UpdatedBy,
                    IsDeleted = x.IsDeleted
                })
                .ToListAsync();

            return new ResponseAllDto<List<AtividadeDto>>(itensPaginados, totalItens, itensPaginados.Count);
        }

        // Método para obter uma atividade específica pelo ID
        public async Task<AtividadeDto> GetById(Guid id) {
            var atividade = _uow.AtividadeRepository
                .Find(x => !x.IsDeleted && x.IdAtividade == id)
                .FirstOrDefault();

            return _mapper.Map<AtividadeDto>(atividade);
        }

        // Método para inserir ou atualizar uma atividade
        public async Task<AtividadeDto> Upsert(AtividadeDto atividadeDto, UserDto currentUser) {
            try {
                // Procura pela atividade utilizando a propriedade IdAtividade
                var atividade = _uow.AtividadeRepository
                    .Find(p => p.IdAtividade == atividadeDto.IdAtividade)
                    .FirstOrDefault();

                bool insert = false;
                if (atividade == null) {
                    insert = true;
                    atividade = new Atividade { IdAtividade = Guid.NewGuid() };
                }

                // Atualiza as propriedades conforme o novo DTO
                atividade.IdAtividadeFilho = atividadeDto.IdAtividadeFilho;
                atividade.Nome = atividadeDto.Nome;
                atividade.DataInicio = atividadeDto.DataInicio;
                atividade.DataFim = atividadeDto.DataFim;
                atividade.Status = atividadeDto.Status;
                atividade.Progresso = atividadeDto.Progresso;
                atividade.HorasEstimadas = atividadeDto.HorasEstimadas;
                atividade.HorasTotais = atividadeDto.HorasTotais;
                atividade.Cobrado = atividadeDto.Cobrado;

                if (insert) {
                    atividade.Created = DateTimeOffset.UtcNow;
                    atividade.CreatedBy = currentUser != null ? new Guid(currentUser.Id) : Guid.NewGuid();
                    atividade.IsDeleted = false;
                    _uow.AtividadeRepository.Insert(atividade);
                }
                else {
                    atividade.Updated = DateTimeOffset.UtcNow;
                    atividade.UpdatedBy = currentUser != null ? new Guid(currentUser.Id) : Guid.NewGuid();
                    atividade.IsDeleted = false;
                    _uow.AtividadeRepository.Update(atividade);
                }

                await _uow.Save();
                return _mapper.Map<AtividadeDto>(atividade);
            }
            catch {
                // Em caso de erro, pode ser interessante registrar a exceção e retornar uma notificação
                return null;
            }
        }

        // Método para deletar logicamente uma atividade (soft delete)
        public async Task<bool> Delete(Guid id, UserDto currentUser) {
            var atividade = _uow.AtividadeRepository
                .Find(x => x.IdAtividade == id)
                .FirstOrDefault();

            if (atividade != null) {
                atividade.Updated = DateTimeOffset.UtcNow;
                atividade.UpdatedBy = new Guid(currentUser.Id);
                atividade.IsDeleted = true;
                _uow.AtividadeRepository.Update(atividade);
                await _uow.Save();
                return true;
            }
            else {
                return false;
            }
        }
    }
}
