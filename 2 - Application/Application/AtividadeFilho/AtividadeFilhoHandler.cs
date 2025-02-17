// Define a classe AtividadeFilhoHandler que contém métodos para lidar com operações CRUD (Criar, Ler, Atualizar, Deletar)
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

namespace Application.AtividadeFilhoHandler
{
    public class AtividadeFilhoHandler
    {
        // Variáveis privadas para injeção de dependência do UnitOfWork, Mapper, Notification, e Configuration
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly INotification _notification;
        private readonly string _getAzureStorageConnectionString;
        private readonly string _getAzureBlobStorageContainer;

        // Construtor para injetar dependências necessárias, como UnitOfWork, Mapper, Notification e Configuration
        public AtividadeFilhoHandler(IUnitOfWork uow, IMapper mapper, INotification notification, IConfiguration configuration)
        {
            _uow = uow;
            _mapper = mapper;
            _notification = notification;
            _configuration = configuration;
        }

        // Método para upload de arquivos no Azure Blob Storage
        public async Task<string> UploadFile(FileUploadRequest request)
        {
            try
            {
                // Instancia um objeto de BlobStorage e busca o arquivo pelo identificador (key)
                var blobStorage = new BlobStorage(_configuration);
                var file = request.Files.FirstOrDefault();
                var fileExtension = Path.GetExtension(file.FileName);
                var newFilename = Guid.NewGuid().ToString() + fileExtension;

                await blobStorage.UploadFile(file.OpenReadStream(), newFilename, file.ContentType);
                return newFilename; // Retorna o nome do blob salvo
            }
            catch
            {
                return null; // Se ocorrer um erro, retorna null
            }
        }

        // Método para buscar um arquivo no Azure Blob Storage
        public async Task<(Stream, string)> GetFile(string key)
        {
            try
            {
                // Instancia um objeto de BlobStorage e busca o arquivo pelo identificador (key)
                var blobStorage = new BlobStorage(_configuration);
                var (fileStream, contentType) = await blobStorage.GetFile(key);

                return (fileStream, contentType); // Retorna o stream do arquivo e o tipo de conteúdo
            }
            catch
            {
                return (null, null); // Se ocorrer um erro, retorna null
            }
        }

        public async Task<bool> RemoveFile(string filename)
        {
            try
            {
                // Instancia um objeto de BlobStorage e busca o arquivo pelo identificador (key)
                var blobStorage = new BlobStorage(_configuration);
                await blobStorage.RemoveFile(filename);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Método para obter uma lista paginada de registros CRUD
        //public async Task<ResponseAllDto<List<AtividadeFilhoDto>>> GetAll(RequestAllAtividadeFilhoDto request)
        //{
        //    // Cria uma consulta base para buscar registros que não estão deletados
        //    var consultaBase = _uow.AtividadeFilhoRepository.Find(x => !x.IsDeleted).AsQueryable();

        //    // Aplica os filtros dinamicamente usando Expressões Lambda
        //    consultaBase = consultaBase.ApplyFilters(request);

        //    // Aplica a ordenação dinâmica conforme os parâmetros fornecidos
        //    if (!string.IsNullOrEmpty(request.SortOrder) && !string.IsNullOrEmpty(request.SorterField))
        //        consultaBase = consultaBase.ApplySorting(request.SorterField, request.SortOrder);
        //    else
        //        consultaBase = consultaBase.OrderByDescending(x => x.Updated).ThenByDescending(x => x.Created);

        //    // Conta o total de itens disponíveis na consulta
        //    var totalItens = await consultaBase.CountAsync();

        //    // Realiza a paginação conforme os parâmetros fornecidos
        //    var itensPaginados = await consultaBase
        //        .Skip((request.Page - 1) * request.PageSize)
        //        .Take(request.PageSize)
        //        .Select(x => new AtividadeFilhoDto
        //        {
        //            Id = x.Id,
        //            Text = x.Text,
        //            Integer = x.Integer
        //        })
        //        .ToListAsync();

        //    // Retorna uma resposta com os itens paginados e o total de itens
        //    return new ResponseAllDto<List<AtividadeFilhoDto>>(itensPaginados, totalItens, itensPaginados.Count);
        //}

        //GPT
        public async Task<ResponseAllDto<List<AtividadeFilhoDto>>> GetAll(RequestAllAtividadeFilhoDto request) {
            var consultaBase = _uow.AtividadeFilhoRepository.Find(x => !x.IsDeleted && x.IdAtividadePai == request.IdAtividadePai).AsQueryable();
            //consultaBase = consultaBase.ApplyFilters(request);

            if (!string.IsNullOrEmpty(request.SortOrder) && !string.IsNullOrEmpty(request.SorterField))
                consultaBase = consultaBase.ApplySorting(request.SorterField, request.SortOrder);
            else
                consultaBase = consultaBase.OrderByDescending(x => x.Updated).ThenByDescending(x => x.Created);

            var totalItens = await consultaBase.CountAsync();
            var itensPaginados = await consultaBase
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => _mapper.Map<AtividadeFilhoDto>(x))
                .ToListAsync();

            return new ResponseAllDto<List<AtividadeFilhoDto>>(itensPaginados, totalItens, itensPaginados.Count);
        }

        // Método para obter um item especifico de registros CRUD
        public async Task<AtividadeFilhoDto> GetById(Guid idAtividadeFilho)
        {
            // Cria uma consulta base para buscar registros que não estão deletados
            var consultaBase = _uow.AtividadeFilhoRepository.Find(x => !x.IsDeleted && x.IdAtividadeFilho == idAtividadeFilho).FirstOrDefault();

            // Retorna uma resposta com o item em DTO
            return _mapper.Map<AtividadeFilhoDto>(consultaBase);
        }

        // Método para inserir ou atualizar um registro CRUD
        //public async Task<AtividadeFilhoDto> Upsert(AtividadeFilhoDto atividadeFilhoDto, UserDto currentUser)
        //{
        //    try
        //    {
        //        // Tenta encontrar o registro CRUD pelo ID fornecido
        //        AtividadeFilho atividadeFilho = _uow.AtividadeFilhoRepository.Find(p => p.IdAtividadeFilho == atividadeFilhoDto.IdAtividadeFilho).FirstOrDefault();

        //        var insert = false;
        //        if (atividadeFilho == null)
        //        {
        //            // Se não encontrar, cria um novo registro
        //            insert = true;
        //            atividadeFilho = new AtividadeFilho { IdAtividadeFilho = Guid.NewGuid() };
        //        }

        //        // Atualiza as propriedades do registro
        //        atividadeFilho.Date = atividadeFilhoDto.Date;
        //        atividadeFilho.Decimal = atividadeFilhoDto.Decimal;
        //        atividadeFilho.Files = atividadeFilhoDto.Files;
        //        atividadeFilho.Integer = atividadeFilhoDto.Integer;
        //        atividadeFilho.MultiSelect = atividadeFilhoDto.MultiSelect;
        //        atividadeFilho.Text = atividadeFilhoDto.Text;
        //        atividadeFilho.TextArea = atividadeFilhoDto.TextArea;
        //        atividadeFilho.Time = atividadeFilhoDto.Time;

        //        // Se for uma inserção, define os campos de criação
        //        if (insert)
        //        {
        //            atividadeFilho.Created = DateTime.UtcNow;
        //            atividadeFilho.CreatedBy = currentUser != null ? new Guid(currentUser.Id) : Guid.NewGuid(); // Define o usuário atual ou gera um novo GUID
        //            atividadeFilho.IsDeleted = false; // Marca o registro como não deletado
        //            _uow.AtividadeFilhoRepository.Insert(atividadeFilho); // Insere o novo registro
        //        }
        //        else
        //        {
        //            // Se for uma atualização, define os campos de atualização
        //            atividadeFilho.Updated = DateTime.UtcNow;
        //            atividadeFilho.UpdatedBy = currentUser != null ? new Guid(currentUser.Id) : Guid.NewGuid(); // Define o usuário atual ou gera um novo GUID
        //            atividadeFilho.IsDeleted = false; // Marca o registro como não deletado
        //            _uow.AtividadeFilhoRepository.Update(atividadeFilho); // Atualiza o registro existente
        //        }

        //        await _uow.Save(); // Salva as alterações no banco de dados
        //        return _mapper.Map<AtividadeFilhoDto>(atividadeFilho); // Mapeia e retorna o DTO atualizado
        //    }
        //    catch
        //    {
        //        return null; // Se ocorrer um erro, retorna null
        //    }
        //}

        // Método para deletar logicamente um registro (soft delete)
        //GPT
        public async Task<AtividadeFilhoDto> Upsert(AtividadeFilhoDto atividadeFilhoDto, UserDto currentUser) {
            try {
                var atividadeFilho = _uow.AtividadeFilhoRepository.Find(p => p.IdAtividadeFilho == atividadeFilhoDto.IdAtividadeFilho).FirstOrDefault();
                bool insert = atividadeFilho == null;

                if (insert) {
                    atividadeFilho = new AtividadeFilho { IdAtividadeFilho = Guid.NewGuid() };
                }

                atividadeFilho.IdAtividadePai = atividadeFilhoDto.IdAtividadePai;
                atividadeFilho.Nome = atividadeFilhoDto.Nome;
                atividadeFilho.DataInicio = atividadeFilhoDto.DataInicio;
                atividadeFilho.DataFim = atividadeFilhoDto.DataFim;
                atividadeFilho.Progresso = atividadeFilhoDto.Progresso;
                atividadeFilho.HorasEstimadas = atividadeFilhoDto.HorasEstimadas;
                atividadeFilho.HorasCobradas = atividadeFilhoDto.HorasCobradas;
                atividadeFilho.HorasNaoCobradas = atividadeFilhoDto.HorasNaoCobradas;
                atividadeFilho.Descricao = atividadeFilhoDto.Descricao;
                atividadeFilho.Files = atividadeFilhoDto.Files;

                if (insert) {
                    atividadeFilho.Created = DateTime.UtcNow;
                    atividadeFilho.CreatedBy = new Guid(currentUser.Id);
                    atividadeFilho.IsDeleted = false;
                    _uow.AtividadeFilhoRepository.Insert(atividadeFilho);
                }
                else {
                    atividadeFilho.Updated = DateTime.UtcNow;
                    atividadeFilho.UpdatedBy = new Guid(currentUser.Id);
                    _uow.AtividadeFilhoRepository.Update(atividadeFilho);
                }

                await _uow.Save();
                return _mapper.Map<AtividadeFilhoDto>(atividadeFilho);
            }
            catch {
                return null;
            }
        }
        public async Task<bool> Delete(Guid idAtividadeFilho, UserDto currentUser)
        {
            // Busca o registro pelo ID
            var generic = _uow.AtividadeFilhoRepository.Find(x => x.IdAtividadeFilho == idAtividadeFilho).FirstOrDefault();
            if (generic != null)
            {
                // Se encontrar o registro, atualiza os campos de atualização e marca como deletado
                generic.Updated = DateTime.UtcNow;
                generic.UpdatedBy = new Guid(currentUser.Id); // Define o usuário atual como quem atualizou
                generic.IsDeleted = true; // Marca como deletado
                _uow.AtividadeFilhoRepository.Update(generic); // Atualiza o registro no banco de dados
                await _uow.Save(); // Salva as alterações
                return true; // Retorna sucesso
            }
            else
            {
                return false; // Se não encontrar o registro, retorna falso
            }
        }
    }
}
