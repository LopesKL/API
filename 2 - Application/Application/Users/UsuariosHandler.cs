using API.Application.Dto;
using API.Application.Dto.ResponsePatterns;
using API.Domain.Interfaces.Write;
using API.Domain.Notifications;
using API.Domain.Users.Auth;
using API.Domain.Users.Auth.JWT;
using Application;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static API.Application.Dto.UserDto;
using INotification = API.Domain.Notifications.INotificationHandler;

namespace API.Application.Users {
    public class UsuariosHandler {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly TokenConfigurations _tokenConfigurations;
        private readonly SigningConfigurations _signingConfigurations;
        private readonly IMapper _mapper;
        private readonly INotification _notification;
        private List<RoleDto> roles;

        public UsuariosHandler(IUnitOfWork uow
            , UserManager<AppUser> userManager
            , RoleManager<AppRole> roleManager
            , SignInManager<AppUser> signInManager
            , TokenConfigurations tokenConfigurations
            , SigningConfigurations signingConfigurations
            , IMapper mapper
            , INotification notification) {
            _uow = uow;
            _mapper = mapper;
            _signInManager = signInManager;
            _tokenConfigurations = tokenConfigurations;
            _signingConfigurations = signingConfigurations;
            _userManager = userManager;
            _roleManager = roleManager;
            _notification = notification;
        }


        // Método para buscar todos os usuários
        public async Task<ResponseAllDto<List<UserDto>>> GetAll(RequestAllUserDto request, UserDto currentUser) {

            // Cria uma consulta base para buscar registros que não estão deletados
            var consultaBase = _userManager.Users.Where(w => w.Active
            );

            // Aplica os filtros dinamicamente usando Expressões Lambda
            consultaBase = consultaBase.ApplyFilters(request);

            // Aplica a ordenação dinâmica conforme os parâmetros fornecidos
            if (!string.IsNullOrEmpty(request.SortOrder) && !string.IsNullOrEmpty(request.SorterField))
                consultaBase = consultaBase.ApplySorting(request.SorterField, request.SortOrder);
            else
                consultaBase = consultaBase.OrderByDescending(x => x.Created);

            // Conta o total de itens disponíveis na consulta
            var totalItens = await consultaBase.CountAsync();

            // Calcula o número de itens a serem ignorados para a paginação
            var skip = (request.Page - 1) * request.PageSize;
            if (skip < 0) skip = 0;
            // Realiza a paginação conforme os parâmetros fornecidos
            var itensPaginados = await consultaBase
                .Skip(skip)
                .Take(request.PageSize)
                .Select(x => new UserDto {
                    Id = new (x.Id),
                    UserName = x.UserName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                })
                 .AsNoTracking().ToListAsync();


            // Retorna uma resposta com os itens paginados e o total de itens
            return new ResponseAllDto<List<UserDto>>(itensPaginados, totalItens, itensPaginados.Count);

        }

        public async Task<ResponseAllDto<List<UserDto>>> GetAllMotorista(RequestAllUserDto request) {

            // Cria uma consulta base para buscar registros que não estão deletados
            var consultaBase = _userManager.Users.Where(w => w.Active);

            // Aplica os filtros dinamicamente usando Expressões Lambda
            consultaBase = consultaBase.ApplyFilters(request);

            // Aplica a ordenação dinâmica conforme os parâmetros fornecidos
            if (!string.IsNullOrEmpty(request.SortOrder) && !string.IsNullOrEmpty(request.SorterField))
                consultaBase = consultaBase.ApplySorting(request.SorterField, request.SortOrder);
            else
                consultaBase = consultaBase.OrderByDescending(x => x.Created);

            // Conta o total de itens disponíveis na consulta
            var totalItens = await consultaBase.CountAsync();

            // Realiza a paginação conforme os parâmetros fornecidos
            var itensPaginados = await consultaBase
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new UserDto {
                    Id = new(x.Id),
                    FirstName = x.UserName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                })
                .ToListAsync();

            // Retorna uma resposta com os itens paginados e o total de itens
            return new ResponseAllDto<List<UserDto>>(itensPaginados, totalItens, itensPaginados.Count);

        }
        public async Task<UserDto> GetUserByLogin(string key) {
            var appUser = await _userManager.FindByNameAsync(key);
            if (appUser == null) {
                _notification.DefaultBuilder()
                    .Code("GetUserByLogin_01")
                    .Message("User not found.")
                    .RaiseNotification();

                return null;
            }

            var roles = await _userManager.GetRolesAsync(appUser);
            if (roles == null || !roles.Any()) {
                _notification.DefaultBuilder()
                    .Code("GetUserByLogin_02")
                    .Message("User has no permissions in the system.")
                    .RaiseNotification();

                return null;
            }

            var rolesList = new List<AppRole>();

            foreach (var roleName in roles) {
                var role = await _roleManager.FindByNameAsync(roleName);
                rolesList.Add(role);
            }

            var result = _mapper.Map<UserDto>(appUser);
            var rolesUser = rolesList.Select(role => new RoleDto {
                Id = role.Id,
                Name = role.Name
            }).ToList();

            result.Roles = rolesUser;

            return result;
        }
        // Método para buscar um usuário por ID
        public async Task<UserDto> GetById(string id) {
            // Cria uma consulta base para buscar registros que não estão deletados
            var consultaBase = await _userManager.FindByIdAsync(id);

            // Retorna uma resposta com o item em DTO
            return _mapper.Map<UserDto>(consultaBase);
        }

        // Método para inserir ou atualizar um usuário
        public async Task<UserDto> Upsert(UserDto userDto, UserDto currentUser) {
            try {
                AppUser userRequest = null;

                // Se o Id estiver preenchido e for um Guid válido, busca pelo Id; caso contrário, busca pelo UserName
                if (!string.IsNullOrEmpty(userDto.Id) && Guid.TryParse(userDto.Id, out Guid userId) && userId != Guid.Empty) {
                    userRequest = await _userManager.FindByIdAsync(userDto.Id);
                }
                if (userRequest == null) {
                    userRequest = await _userManager.FindByNameAsync(userDto.UserName);
                }

                var isNewUser = userRequest == null;

                if (isNewUser) {
                    // Verifica se o nome de usuário já existe
                    var usernameExists = await _userManager.FindByNameAsync(userDto.UserName);
                    if (usernameExists != null) {
                        _notification.DefaultBuilder()
                            .Code("InsertUser_02")
                            .Message("This username already exists.")
                            .RaiseNotification();
                        return null;
                    }

                    // Cria um novo usuário e atribui os valores do DTO
                    userRequest = new AppUser {
                        Id = Guid.NewGuid().ToString(),
                        UserName = userDto.UserName,
                        Email = userDto.Email,
                        PhoneNumber = userDto.PhoneNumber,
                        Active = userDto.Active,
                        TaxaFaturamento = userDto.TaxaFaturamento,
                        Empresa = userDto.Empresa,
                        Created = DateTime.UtcNow, // ou userDto.Created, se for permitido
                        CreatedBy = currentUser != null && Guid.TryParse(currentUser.Id, out Guid currentUserId)
                                        ? currentUserId
                                        : Guid.NewGuid(),
                    };

                    var createResult = await _userManager.CreateAsync(userRequest, userDto.Password);
                    if (!createResult.Succeeded) {
                        // Aqui você pode incluir a notificação dos erros retornados
                        return null;
                    }

                    // Se o DTO possuir roles definidas, as atribui ao usuário recém-criado
                    if (userDto.Roles != null && userDto.Roles.Any()) {
                        // Supondo que cada RoleDto possua a propriedade "Name"
                        var roles = userDto.Roles.Select(r => r.Name).ToList();
                        await _userManager.AddToRolesAsync(userRequest, roles);
                    }
                }
                else {
                    // Atualiza os dados do usuário com as informações do DTO
                    userRequest.Email = userDto.Email;
                    userRequest.UserName = userDto.UserName;
                    userRequest.PhoneNumber = userDto.PhoneNumber;
                    userRequest.Active = userDto.Active;
                    userRequest.TaxaFaturamento = userDto.TaxaFaturamento;
                    userRequest.Empresa = userDto.Empresa;
                    // Se necessário, atualiza o campo LastLogin (verifique se este campo pode ser alterado nesta operação)
                    if (userDto.LastLogin.HasValue) {
                        userRequest.LastLogin = userDto.LastLogin.Value;
                    }
                    userRequest.Updated = DateTime.UtcNow; // ou userDto.Updated, conforme a regra de negócio
                    userRequest.UpdatedBy = currentUser != null && Guid.TryParse(currentUser.Id, out Guid currentUserId)
                                                ? currentUserId
                                                : Guid.NewGuid();

                    var updateResult = await _userManager.UpdateAsync(userRequest);
                    if (!updateResult.Succeeded) {
                        // Notifique os erros, se necessário
                        return null;
                    }

                    // Atualiza as roles: remove as atuais e adiciona as novas (caso o DTO possua roles)
                    if (userDto.Roles != null && userDto.Roles.Any()) {
                        var currentRoles = await _userManager.GetRolesAsync(userRequest);
                        await _userManager.RemoveFromRolesAsync(userRequest, currentRoles);
                        var newRoles = userDto.Roles.Select(r => r.Name).ToList();
                        await _userManager.AddToRolesAsync(userRequest, newRoles);
                    }
                }

                // Atualiza a senha caso ela tenha sido fornecida
                if (!string.IsNullOrEmpty(userDto.Password)) {
                    // Remove a senha atual (se existir) e adiciona a nova
                    var removePassResult = await _userManager.RemovePasswordAsync(userRequest);
                    if (!removePassResult.Succeeded) {
                        return null;
                    }
                    var addPassResult = await _userManager.AddPasswordAsync(userRequest, userDto.Password);
                    if (!addPassResult.Succeeded) {
                        return null;
                    }
                }

                // Salva as alterações no banco de dados (por meio da UnitOfWork, por exemplo)
                await _uow.Save();

                // Retorna o usuário atualizado mapeado para o DTO
                return _mapper.Map<UserDto>(userRequest);
            }
            catch (Exception ex) {
                _notification.DefaultBuilder()
                    .Code("UpsertException")
                    .Message($"Erro ao processar a operação: {ex.Message}")
                    .RaiseNotification();
                return null;
            }
        }


        private async Task AssignRoles(AppUser userRequest) {
                await _userManager.AddToRoleAsync(userRequest, Roles.ROLE_ADMIN);
        }

        // Método para deletar um usuário
        public async Task<bool> Delete(Guid id, UserDto currentUser) {
            // Busca o registro pelo ID
            var generic = await _userManager.FindByIdAsync(id.ToString());
            if (generic != null) {
                // Se encontrar o registro, atualiza os campos de atualização e marca como deletado
                generic.Updated = DateTime.UtcNow;
                generic.Active = false; // Marca como deletado
                await _userManager.UpdateAsync(generic); // Atualiza o registro no banco de dados
                await _uow.Save(); // Salva as alterações
                return true; // Retorna sucesso
            }
            else {
                return false; // Se não encontrar o registro, retorna falso
            }
        }
    }
}
