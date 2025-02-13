//// Define a classe CrudHandler que contém métodos para lidar com operações CRUD (Criar, Ler, Atualizar, Deletar)
//using API.Application.Blobs;
//using API.Application.Dto;
//using API.Application.Dto.Request;
//using API.Application.Dto.ResponsePatterns;
//using API.Domain.Interfaces.Write;
//using API.Domain.Projeto;
//using AutoMapper;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using INotification = API.Domain.Notifications.INotificationHandler;

//namespace Application.CrudHandler {
//    public class CrudHandler {
//        // Variáveis privadas para injeção de dependência do UnitOfWork, Mapper, Notification, e Configuration
//        private readonly IUnitOfWork _uow;
//        private readonly IMapper _mapper;
//        private readonly IConfiguration _configuration;
//        private readonly INotification _notification;
//        private readonly string _getAzureStorageConnectionString;
//        private readonly string _getAzureBlobStorageContainer;

//        // Construtor para injetar dependências necessárias, como UnitOfWork, Mapper, Notification e Configuration
//        public CrudHandler(IUnitOfWork uow, IMapper mapper, INotification notification, IConfiguration configuration) {
//            _uow = uow;
//            _mapper = mapper;
//            _notification = notification;
//            _configuration = configuration;
//        }

//        // Método para upload de arquivos no Azure Blob Storage
//        public async Task<string> UploadFile(FileUploadRequest request) {
//            try {
//                // Instancia um objeto de BlobStorage e busca o arquivo pelo identificador (key)
//                var blobStorage = new BlobStorage(_configuration);
//                var file = request.Files.FirstOrDefault();
//                var fileExtension = Path.GetExtension(file.FileName);
//                var newFilename = Guid.NewGuid().ToString() + fileExtension;

//                await blobStorage.UploadFile(file.OpenReadStream(), newFilename, file.ContentType);
//                return newFilename; // Retorna o nome do blob salvo
//            }
//            catch {
//                return null; // Se ocorrer um erro, retorna null
//            }
//        }

//        // Método para buscar um arquivo no Azure Blob Storage
//        public async Task<(Stream, string)> GetFile(string key) {
//            try {
//                // Instancia um objeto de BlobStorage e busca o arquivo pelo identificador (key)
//                var blobStorage = new BlobStorage(_configuration);
//                var (fileStream, contentType) = await blobStorage.GetFile(key);

//                return (fileStream, contentType); // Retorna o stream do arquivo e o tipo de conteúdo
//            }
//            catch {
//                return (null, null); // Se ocorrer um erro, retorna null
//            }
//        }

//        public async Task<bool> RemoveFile(string filename) {
//            try {
//                // Instancia um objeto de BlobStorage e busca o arquivo pelo identificador (key)
//                var blobStorage = new BlobStorage(_configuration);
//                await blobStorage.RemoveFile(filename);
//                return true;
//            }
//            catch {
//                return false;
//            }
//        }

//        // Método para obter uma lista paginada de registros CRUD
//        public async Task<ResponseAllDto<List<CrudDto>>> GetAll(RequestAllCrudDto request) {
//            // Cria uma consulta base para buscar registros que não estão deletados
//            var consultaBase = _uow.CrudRepository.Find(x => !x.IsDeleted).AsQueryable();

//            // Aplica os filtros dinamicamente usando Expressões Lambda
//            consultaBase = consultaBase.ApplyFilters(request);

//            // Aplica a ordenação dinâmica conforme os parâmetros fornecidos
//            if (!string.IsNullOrEmpty(request.SortOrder) && !string.IsNullOrEmpty(request.SorterField))
//                consultaBase = consultaBase.ApplySorting(request.SorterField, request.SortOrder);
//            else
//                consultaBase = consultaBase.OrderByDescending(x => x.Updated).ThenByDescending(x => x.Created);

//            // Conta o total de itens disponíveis na consulta
//            var totalItens = await consultaBase.CountAsync();

//            // Realiza a paginação conforme os parâmetros fornecidos
//            var itensPaginados = await consultaBase
//                .Skip((request.Page - 1) * request.PageSize)
//                .Take(request.PageSize)
//                .Select(x => new CrudDto {
//                    Id = x.Id,
//                    Text = x.Text,
//                    Integer = x.Integer
//                })
//                .ToListAsync();

//            // Retorna uma resposta com os itens paginados e o total de itens
//            return new ResponseAllDto<List<CrudDto>>(itensPaginados, totalItens, itensPaginados.Count);
//        }

//        // Método para obter um item especifico de registros CRUD
//        public async Task<CrudDto> GetById(Guid id) {
//            // Cria uma consulta base para buscar registros que não estão deletados
//            var consultaBase = _uow.CrudRepository.Find(x => !x.IsDeleted && x.Id == id).FirstOrDefault();

//            // Retorna uma resposta com o item em DTO
//            return _mapper.Map<CrudDto>(consultaBase);
//        }

//        // Método para inserir ou atualizar um registro CRUD
//        public async Task<CrudDto> Upsert(CrudDto crudDto, UserDto currentUser) {
//            try {
//                // Tenta encontrar o registro CRUD pelo ID fornecido
//                Crud crud = _uow.CrudRepository.Find(p => p.Id == crudDto.Id).FirstOrDefault();

//                var insert = false;
//                if (crud == null) {
//                    // Se não encontrar, cria um novo registro
//                    insert = true;
//                    crud = new Crud { Id = Guid.NewGuid() };
//                }

//                // Atualiza as propriedades do registro
//                crud.Date = crudDto.Date;
//                crud.Decimal = crudDto.Decimal;
//                crud.Files = crudDto.Files;
//                crud.Integer = crudDto.Integer;
//                crud.MultiSelect = crudDto.MultiSelect;
//                crud.Select = crudDto.Select;
//                crud.Text = crudDto.Text;
//                crud.TextArea = crudDto.TextArea;
//                crud.Time = crudDto.Time;

//                // Se for uma inserção, define os campos de criação
//                if (insert) {
//                    crud.Created = DateTime.UtcNow;
//                    crud.CreatedBy = currentUser != null ? new Guid(currentUser.Id) : Guid.NewGuid(); // Define o usuário atual ou gera um novo GUID
//                    crud.IsDeleted = false; // Marca o registro como não deletado
//                    _uow.CrudRepository.Insert(crud); // Insere o novo registro
//                }
//                else {
//                    // Se for uma atualização, define os campos de atualização
//                    crud.Updated = DateTime.UtcNow;
//                    crud.UpdatedBy = currentUser != null ? new Guid(currentUser.Id) : Guid.NewGuid(); // Define o usuário atual ou gera um novo GUID
//                    crud.IsDeleted = false; // Marca o registro como não deletado
//                    _uow.CrudRepository.Update(crud); // Atualiza o registro existente
//                }

//                await _uow.Save(); // Salva as alterações no banco de dados
//                return _mapper.Map<CrudDto>(crud); // Mapeia e retorna o DTO atualizado
//            }
//            catch {
//                return null; // Se ocorrer um erro, retorna null
//            }
//        }

//        // Método para deletar logicamente um registro (soft delete)
//        public async Task<bool> Delete(Guid id, UserDto currentUser) {
//            // Busca o registro pelo ID
//            var generic = _uow.CrudRepository.Find(x => x.Id == id).FirstOrDefault();
//            if (generic != null) {
//                // Se encontrar o registro, atualiza os campos de atualização e marca como deletado
//                generic.Updated = DateTime.UtcNow;
//                generic.UpdatedBy = new Guid(currentUser.Id); // Define o usuário atual como quem atualizou
//                generic.IsDeleted = true; // Marca como deletado
//                _uow.CrudRepository.Update(generic); // Atualiza o registro no banco de dados
//                await _uow.Save(); // Salva as alterações
//                return true; // Retorna sucesso
//            }
//            else {
//                return false; // Se não encontrar o registro, retorna falso
//            }
//        }
//    }
//}
