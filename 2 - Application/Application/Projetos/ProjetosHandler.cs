// Define a classe ProjetoHandler que contém métodos para lidar com operações CRUD (Criar, Ler, Atualizar, Deletar)
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

namespace Application.PropjetosHandler {
    public class ProjetosHandler {
        // Variáveis privadas para injeção de dependência do UnitOfWork, Mapper, Notification, e Configuration
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly INotification _notification;
        private readonly string _getAzureStorageConnectionString;
        private readonly string _getAzureBlobStorageContainer;

        // Construtor para injetar dependências necessárias, como UnitOfWork, Mapper, Notification e Configuration
        public ProjetosHandler(IUnitOfWork uow, IMapper mapper, INotification notification, IConfiguration configuration) {
            _uow = uow;
            _mapper = mapper;
            _notification = notification;
            _configuration = configuration;
        }

        // Método para upload de arquivos no Azure Blob Storage
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

        // Método para obter uma lista paginada de registros CRUD
        //public async Task<ResponseAllDto<List<ProjetosDto>>> GetAll(RequestAllProjetosDto request) {
        //    // Cria uma consulta base para buscar registros que não estão deletados
        //    var consultaBase = _uow.ProjetosRepository.Find(x => !x.IsDeleted).AsQueryable();

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
        //        .Select(x => new ProjetosDto {
        //            IdProjetos = x.IdProjetos,
        //            Text = x.Text,
        //            Integer = x.Integer
        //        })
        //        .ToListAsync();

        //    // Retorna uma resposta com os itens paginados e o total de itens
        //    return new ResponseAllDto<List<ProjetosDto>>(itensPaginados, totalItens, itensPaginados.Count);
        //}

        //Modelo GPT
        public async Task<ResponseAllDto<List<ProjetosDto>>> GetAll(RequestAllProjetosDto request) {
            var consultaBase = _uow.ProjetosRepository.Find(x => !x.IsDeleted).AsQueryable();

            //consultaBase = consultaBase.ApplyFilters(request);

            if (!string.IsNullOrEmpty(request.SortOrder) && !string.IsNullOrEmpty(request.SorterField))
                consultaBase = consultaBase.ApplySorting(request.SorterField, request.SortOrder);
            else
                consultaBase = consultaBase.OrderByDescending(x => x.Updated).ThenByDescending(x => x.Created);

            var totalItens = await consultaBase.CountAsync();
            var itensPaginados = await consultaBase
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => _mapper.Map<ProjetosDto>(x))
                .ToListAsync();

            return new ResponseAllDto<List<ProjetosDto>>(itensPaginados, totalItens, itensPaginados.Count);
        }


        // Método para obter um item especifico de registros CRUD
        public async Task<ProjetosDto> GetById(Guid idProjetos) {
            // Cria uma consulta base para buscar registros que não estão deletados
            var consultaBase = _uow.ProjetosRepository.Find(x => !x.IsDeleted && x.IdProjetos == idProjetos).FirstOrDefault();

            // Retorna uma resposta com o item em DTO
            return _mapper.Map<ProjetosDto>(consultaBase);
        }

        // Método para inserir ou atualizar um registro CRUD
        //public async Task<ProjetosDto> Upsert(ProjetosDto projetosDto, UserDto currentUser) {
        //    try {
        //        // Tenta encontrar o registro CRUD pelo ID fornecido
        //        Projetos projetos = _uow.ProjetosRepository.Find(p => p.IdProjetos == projetosDto.IdProjetos).FirstOrDefault();

        //        var insert = false;
        //        if (projetos == null) {
        //            // Se não encontrar, cria um novo registro
        //            insert = true;
        //            projetos = new Projetos { IdProjetos = Guid.NewGuid() };
        //        }

        //        // Atualiza as propriedades do registro
        //        projetos.Date = projetosDto.Date;
        //        projetos.Decimal = projetosDto.Decimal;
        //        projetos.Files = projetosDto.Files;
        //        projetos.Integer = projetosDto.Integer;
        //        projetos.MultiSelect = projetosDto.MultiSelect;
        //        projetos.Text = projetosDto.Text;
        //        projetos.TextArea = projetosDto.TextArea;
        //        projetos.Time = projetosDto.Time;

        //        // Se for uma inserção, define os campos de criação
        //        if (insert) {
        //            projetos.Created = DateTime.UtcNow;
        //            projetos.CreatedBy = currentUser != null ? new Guid(currentUser.Id) : Guid.NewGuid(); // Define o usuário atual ou gera um novo GUID
        //            projetos.IsDeleted = false; // Marca o registro como não deletado
        //            _uow.ProjetosRepository.Insert(projetos); // Insere o novo registro
        //        }
        //        else {
        //            // Se for uma atualização, define os campos de atualização
        //            projetos.Updated = DateTime.UtcNow;
        //            projetos.UpdatedBy = currentUser != null ? new Guid(currentUser.Id) : Guid.NewGuid(); // Define o usuário atual ou gera um novo GUID
        //            projetos.IsDeleted = false; // Marca o registro como não deletado
        //            _uow.ProjetosRepository.Update(projetos); // Atualiza o registro existente
        //        }

        //        await _uow.Save(); // Salva as alterações no banco de dados
        //        return _mapper.Map<ProjetosDto>(projetos); // Mapeia e retorna o DTO atualizado
        //    }
        //    catch {
        //        return null; // Se ocorrer um erro, retorna null
        //    }
        //}

        //Modelo GPT
        public async Task<ProjetosDto> Upsert(ProjetosDto projetosDto, UserDto currentUser) {
            try {
                var projeto = _uow.ProjetosRepository.Find(p => p.IdProjetos == projetosDto.IdProjetos).FirstOrDefault();
                bool insert = projeto == null;

                if (insert) {
                    projeto = new Projetos { IdProjetos = Guid.NewGuid() };
                }

                projeto.IdEmpresa = projetosDto.IdEmpresa;
                projeto.IdCliente = projetosDto.IdCliente;
                projeto.Nome = projetosDto.Nome;
                projeto.OrcamentoInicial = projetosDto.OrcamentoInicial;
                projeto.Gastos = projetosDto.Gastos;
                projeto.SaldoFinal = projetosDto.SaldoFinal;
                projeto.Moeda = projetosDto.Moeda;
                projeto.DataInicio = projetosDto.DataInicio;
                projeto.DataFim = projetosDto.DataFim;
                projeto.HorasEstimadas = projetosDto.HorasEstimadas;
                projeto.HorasCobradas = projetosDto.HorasCobradas;
                projeto.HorasNaoCobradas = projetosDto.HorasNaoCobradas;
                projeto.Cor = projetosDto.Cor;
                projeto.Status = projetosDto.Status;
                projeto.ProjetoConcluido = projetosDto.ProjetoConcluido;
                projeto.Descricao = projetosDto.Descricao;
                projeto.Files = projetosDto.Files;

                if (insert) {
                    projeto.Created = DateTime.UtcNow;
                    //projeto.CreatedBy = new Guid(currentUser.Id);
                    projeto.IsDeleted = false;
                    _uow.ProjetosRepository.Insert(projeto);
                }
                else {
                    projeto.Updated = DateTime.UtcNow;
                    projeto.UpdatedBy = new Guid(currentUser.Id);
                    _uow.ProjetosRepository.Update(projeto);
                }

                await _uow.Save();
                return _mapper.Map<ProjetosDto>(projeto);
            }
            catch {
                return null;
            }
        }

        // Método para deletar logicamente um registro (soft delete)
        public async Task<bool> Delete(Guid id, UserDto currentUser) {
            // Busca o registro pelo ID
            var generic = _uow.ProjetosRepository.Find(x => x.IdProjetos == id).FirstOrDefault();
            if (generic != null) {
                // Se encontrar o registro, atualiza os campos de atualização e marca como deletado
                generic.Updated = DateTime.UtcNow;
                generic.UpdatedBy = new Guid(currentUser.Id); // Define o usuário atual como quem atualizou
                generic.IsDeleted = true; // Marca como deletado
                _uow.ProjetosRepository.Update(generic); // Atualiza o registro no banco de dados
                await _uow.Save(); // Salva as alterações
                return true; // Retorna sucesso
            }
            else {
                return false; // Se não encontrar o registro, retorna falso
            }
        }
    }
}
