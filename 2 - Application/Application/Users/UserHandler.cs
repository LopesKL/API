using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using API.Application.Dto;
using API.Application.Dto.Response;
using API.Application.Dto.ResponsePatterns;
using API.Domain.Interfaces.Write;
using API.Domain.Notifications;
using API.Domain.Users.Auth;
using API.Domain.Users.Auth.JWT;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using INotification = API.Domain.Notifications.INotificationHandler;

namespace API.Application.Users {
    public class UserHandler {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly TokenConfigurations _tokenConfigurations;
        private readonly SigningConfigurations _signingConfigurations;
        private readonly IMapper _mapper;
        private readonly INotification _notification;

        public UserHandler(IUnitOfWork uow
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

        public async Task<bool> SeedAdmin() {
            var find1 = await _userManager.FindByNameAsync("admin");

            if (find1 == null) {
                var _appUserFactory = new AppUserFactory(new NotificationHandler());
                var appUser = _appUserFactory.DefaultBuilder()
                    .Email("admin@tekann.com.br")
                    .UserName("admin")
                    .Active(true)
                    .Raise();

                await _userManager.CreateAsync(appUser, "Tekann.2024");
                await _userManager.AddToRoleAsync(appUser, Roles.ROLE_ADMIN);

                return true;
            }

            return false;
        }

        public async Task<ResponseAllDto<List<UserDto>>> GetAll(string fullName, string userName, string email, int page, int limit) {
            int skipNumber = page * limit;
            var userQuery = _userManager.Users.Where(w => w.Active);
            if (!string.IsNullOrEmpty(userName))
                userQuery = userQuery.Where(w => w.UserName.Contains(userName));
            if (!string.IsNullOrEmpty(email))
                userQuery = userQuery.Where(w => w.Email.Contains(email));

            var users = await userQuery.OrderBy(x => x.UserName).Skip(skipNumber).Take(limit).ToListAsync();
            var result = new List<UserDto>();

            foreach (var user in users) {
                var userInfo = _mapper.Map<UserDto>(user);
                var rolesAux = await _userManager.GetRolesAsync(user);

                RoleDto roleDto = rolesAux != null && rolesAux.Count() > 0 && rolesAux.Contains(Roles.ROLE_ADMIN)
                    ? new RoleDto { Name = "System Admin" }
                    : new RoleDto { Name = "No permission" };

                userInfo.Roles = new List<RoleDto> { roleDto };
                result.Add(userInfo);
            }

            result = result.OrderBy(x => x.UserName).ToList();
            int totalRecords = await _userManager.Users.Where(w => w.Active).CountAsync();
            var resultado = new ResponseAllDto<List<UserDto>>(result, totalRecords, result.Count);
            return resultado;
        }

        public async Task<UserDto> GetById(Guid key) {
            var appUser = await _userManager.FindByIdAsync(key.ToString());
            if (appUser == null) {
                _notification.DefaultBuilder()
                    .Code("GetUserByLogin_01")
                    .Message("User not found.")
                    .RaiseNotification();

                return null;
            }

            var roles = await _userManager.GetRolesAsync(appUser);
            if (roles == null) {
                _notification.DefaultBuilder()
                    .Code("GetUserByLogin_02")
                    .Message("User has no permissions in the system.")
                    .RaiseNotification();

                return null;
            }

            List<AppRole> rolesList = new List<AppRole>();
            foreach (var role in roles) {
                var approle = await _roleManager.FindByNameAsync(role);
                rolesList.Add(approle);
            }

            var result = _mapper.Map<UserDto>(appUser);

            var rolesUser = rolesList.Select(role => new RoleDto {
                Id = role.Id,
                Name = role.Name
            }).ToList();

            result.Roles = rolesUser;

            return result;
        }

        bool IsValidEmail(string email) {
            try {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch {
                return false;
            }
        }

        public async Task<bool> Insert(UserDto user) {
            if (!string.IsNullOrEmpty(user.Email)) {
                if (!IsValidEmail(user.Email)) {
                    _notification.DefaultBuilder()
                        .Code("UpdatePassword_01")
                        .Message("The email is not in a valid format.")
                        .RaiseNotification();

                    return false;
                }
            }

            var existingUser = user.Id != "0" ? await _userManager.FindByIdAsync(user.Id) : await _userManager.FindByNameAsync(user.UserName);

            if (existingUser != null) {
                existingUser.Email = user.Email;
                existingUser.UserName = user.UserName;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.Active = true;

                var updateResult = await _userManager.UpdateAsync(existingUser);
                if (!updateResult.Succeeded) {
                    foreach (var error in updateResult.Errors) {
                        _notification.DefaultBuilder()
                            .Code(error.Code)
                            .Message(error.Description)
                            .RaiseNotification();
                    }

                    return false;
                }

                if (!string.IsNullOrEmpty(user.Password)) {
                    await _userManager.RemovePasswordAsync(existingUser);
                    var addPassResponse = await _userManager.AddPasswordAsync(existingUser, user.Password);
                    if (!addPassResponse.Succeeded) {
                        foreach (var error in addPassResponse.Errors) {
                            _notification.DefaultBuilder()
                                .Code(error.Code)
                                .Message(error.Description)
                                .RaiseNotification();
                        }

                        return false;
                    }
                }

                var actualRoles = await _userManager.GetRolesAsync(existingUser);
                var rolesId = user.Roles.Select(s => s.Id).ToList();
                var lstRoles = await _uow.AppRoleRepository.Find(f => rolesId.Contains(f.Id)).ToListAsync();

                

                if (actualRoles != null && actualRoles.Count > 0)
                    await _userManager.RemoveFromRolesAsync(existingUser, actualRoles);

                foreach (var role in lstRoles) {
                    await _userManager.AddToRoleAsync(existingUser, role.Name);
                }
            }
            else {
                var usernameExists = await _userManager.FindByNameAsync(user.UserName);
                if (usernameExists != null) {
                    _notification.DefaultBuilder()
                        .Code("InsertUser_02")
                        .Message("This username already exists.")
                        .RaiseNotification();

                    return false;
                }

                var appUser = new AppUser {
                    Id = Guid.NewGuid().ToString(),
                    Email = user.Email,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Active = true
                };

                var createResult = await _userManager.CreateAsync(appUser, user.Password);
                if (!createResult.Succeeded) {
                    foreach (var error in createResult.Errors) {
                        _notification.DefaultBuilder()
                            .Code(error.Code)
                            .Message(error.Description)
                            .RaiseNotification();
                    }

                    return false;
                }

                var rolesId = user.Roles.Select(s => s.Id).ToList();
                var lstRoles = await _uow.AppRoleRepository.Find(f => rolesId.Contains(f.Id)).ToListAsync();

                foreach (var role in lstRoles) {
                    await _userManager.AddToRoleAsync(appUser, role.Name);
                }

            }

            return true;
        }

        public async Task<bool> Delete(string userID) {
            var user = await _userManager.FindByIdAsync(userID);

            if (user != null) {
                user.Active = false;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded) {
                    foreach (var error in updateResult.Errors) {
                        _notification.DefaultBuilder()
                            .Code(error.Code)
                            .Message(error.Description)
                            .RaiseNotification();
                    }

                    return false;
                }
            }
            else {
                _notification.DefaultBuilder()
                    .Code("InsertUser_02")
                    .Message("O usuário informado não foi encontrado na base de dados.")
                    .RaiseNotification();

                return false;

            }

            return true;
        }

        public async Task<TokenResultDto> SignIn(UserSignInDto userSignDto) {
            if (string.IsNullOrEmpty(userSignDto.UserName) || string.IsNullOrEmpty(userSignDto.Password)) {
                _notification.DefaultBuilder()
                    .Code("SignIn_01")
                    .Message("Usuário e/ou senha incorretos. Preencha todos os campos para efetuar o login.")
                    .RaiseNotification();

                return null;
            }

            var appUser = await _userManager.FindByNameAsync(userSignDto.UserName);
            if (appUser == null || !appUser.Active) {
                _notification.DefaultBuilder()
                    .Code("SignIn_02")
                    .Message("Usuário e/ou senha incorretos. Verifique se o usuário encontra-se ativo no sistema.")
                    .RaiseNotification();

                return null;
            }

            var passwordOk = await _signInManager.CheckPasswordSignInAsync(appUser, userSignDto.Password, false);
            if (!passwordOk.Succeeded) {
                _notification.DefaultBuilder()
                    .Code("SignIn_03")
                    .Message("Usuário e/ou senha incorretos")
                    .RaiseNotification();

                return null;
            }

            TokenResultDto token = await SetToken(appUser);

            return token;
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

        private async Task<TokenResultDto> SetToken(AppUser appUser, bool partialToken = false, bool integrationWebJobToken = false) {
            var roles = await _userManager.GetRolesAsync(appUser);
            roles = roles.Distinct().ToList();

            if (roles == null || roles.Count == 0) {
                _notification.DefaultBuilder()
                    .Code("SignIn_04")
                    .Message("You are not authorized to access the system.")
                    .RaiseNotification();

                return null;
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.UniqueName, appUser.Id),
                new Claim(ClaimTypes.Email, !string.IsNullOrEmpty(appUser.Email) ? appUser.Email : ""),
                new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString())
            };

            var rolesList = new List<AppRole>();
            foreach (var role in roles) {
                claims.Add(new Claim(ClaimTypes.Role, role));
                var approle = await _roleManager.FindByNameAsync(role);
                rolesList.Add(approle);
            }

            var identity = new ClaimsIdentity(new GenericIdentity(appUser.UserName, "Login"), claims);

            var dateCreate = DateTime.UtcNow;

            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor {
                Issuer = _tokenConfigurations.Issuer,
                Audience = _tokenConfigurations.Audience,
                SigningCredentials = _signingConfigurations.SigningCredentials,
                Subject = identity,
                NotBefore = dateCreate,
                Expires = dateCreate.AddDays(90)
            });

            var token = handler.WriteToken(securityToken);

            var resultReturn = _mapper.Map<UserDto>(appUser);

            var rolesUser = rolesList.Select(role => new RoleDto {
                Id = role.Id,
                Name = role.Name
            }).ToList();

            if (!partialToken) {
                resultReturn.Roles = rolesUser;
            }

            return new TokenResultDto {
                Id = appUser.Id.ToString(),
                Authenticated = true,
                Created = dateCreate,
                Expiration = dateCreate.AddDays(90),
                Roles = roles.ToList(),
                AccessToken = token,
                User = resultReturn,
                Identity = identity
            };
        }
    }
}
