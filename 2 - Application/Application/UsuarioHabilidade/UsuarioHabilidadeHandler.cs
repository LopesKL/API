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

namespace Application.UsuarioHabilidadeHandler {
    public class UsuarioHabilidadeHandler {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly INotification _notification;

        public UsuarioHabilidadeHandler(IUnitOfWork uow, IMapper mapper, INotification notification, IConfiguration configuration) {
            _uow = uow;
            _mapper = mapper;
            _notification = notification;
            _configuration = configuration;
        }

        public async Task<ResponseAllDto<List<UsuarioHabilidadeDto>>> GetAll(RequestAllUsuarioHabilidadeDto request) {
            var consultaBase = _uow.UsuarioHabilidadeRepository.Find(x => !x.IsDeleted).AsQueryable();
            consultaBase = consultaBase.ApplyFilters(request);

            if (!string.IsNullOrEmpty(request.SortOrder) && !string.IsNullOrEmpty(request.SorterField))
                consultaBase = consultaBase.ApplySorting(request.SorterField, request.SortOrder);
            else
                consultaBase = consultaBase.OrderByDescending(x => x.Updated).ThenByDescending(x => x.Created);

            var totalItens = await consultaBase.CountAsync();
            var itensPaginados = await consultaBase
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => _mapper.Map<UsuarioHabilidadeDto>(x))
                .ToListAsync();

            return new ResponseAllDto<List<UsuarioHabilidadeDto>>(itensPaginados, totalItens, itensPaginados.Count);
        }

        public async Task<UsuarioHabilidadeDto> GetById(Guid idUsuario, Guid idHabilidade) {
            var usuarioHabilidade = _uow.UsuarioHabilidadeRepository.Find(x => !x.IsDeleted && x.IdUsuario == idUsuario && x.IdHabilidade == idHabilidade).FirstOrDefault();
            return _mapper.Map<UsuarioHabilidadeDto>(usuarioHabilidade);
        }

        public async Task<UsuarioHabilidadeDto> Upsert(UsuarioHabilidadeDto usuarioHabilidadeDto, UserDto currentUser) {
            try {
                var usuarioHabilidade = _uow.UsuarioHabilidadeRepository.Find(p => p.IdUsuario == usuarioHabilidadeDto.IdUsuario && p.IdHabilidade == usuarioHabilidadeDto.IdHabilidade).FirstOrDefault();
                bool insert = usuarioHabilidade == null;

                if (insert) {
                    usuarioHabilidade = new UsuarioHabilidade {
                        IdUsuario = usuarioHabilidadeDto.IdUsuario,
                        IdHabilidade = usuarioHabilidadeDto.IdHabilidade
                    };
                }

                if (insert) {
                    usuarioHabilidade.Created = DateTime.UtcNow;
                    usuarioHabilidade.CreatedBy = new Guid(currentUser.Id);
                    usuarioHabilidade.IsDeleted = false;
                    _uow.UsuarioHabilidadeRepository.Insert(usuarioHabilidade);
                }
                else {
                    usuarioHabilidade.Updated = DateTime.UtcNow;
                    usuarioHabilidade.UpdatedBy = new Guid(currentUser.Id);
                    _uow.UsuarioHabilidadeRepository.Update(usuarioHabilidade);
                }

                await _uow.Save();
                return _mapper.Map<UsuarioHabilidadeDto>(usuarioHabilidade);
            }
            catch {
                return null;
            }
        }

        public async Task<bool> Delete(Guid idUsuario, Guid idHabilidade, UserDto currentUser) {
            var usuarioHabilidade = _uow.UsuarioHabilidadeRepository.Find(x => x.IdUsuario == idUsuario && x.IdHabilidade == idHabilidade).FirstOrDefault();
            if (usuarioHabilidade != null) {
                usuarioHabilidade.Updated = DateTime.UtcNow;
                usuarioHabilidade.UpdatedBy = new Guid(currentUser.Id);
                usuarioHabilidade.IsDeleted = true;
                _uow.UsuarioHabilidadeRepository.Update(usuarioHabilidade);
                await _uow.Save();
                return true;
            }
            return false;
        }
    }
}
