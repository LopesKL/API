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

namespace Application.ProjetoUsuarioHandler {
    public class ProjetoUsuarioHandler {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly INotification _notification;

        public ProjetoUsuarioHandler(IUnitOfWork uow, IMapper mapper, INotification notification, IConfiguration configuration) {
            _uow = uow;
            _mapper = mapper;
            _notification = notification;
            _configuration = configuration;
        }

        public async Task<ResponseAllDto<List<ProjetoUsuarioDto>>> GetAll(RequestAllProjetoUsuarioDto request) {
            var consultaBase = _uow.ProjetoUsuarioRepository.Find(x => !x.IsDeleted).AsQueryable();
            consultaBase = consultaBase.ApplyFilters(request);

            if (!string.IsNullOrEmpty(request.SortOrder) && !string.IsNullOrEmpty(request.SorterField))
                consultaBase = consultaBase.ApplySorting(request.SorterField, request.SortOrder);
            else
                consultaBase = consultaBase.OrderByDescending(x => x.Updated).ThenByDescending(x => x.Created);

            var totalItens = await consultaBase.CountAsync();
            var itensPaginados = await consultaBase
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => _mapper.Map<ProjetoUsuarioDto>(x))
                .ToListAsync();

            return new ResponseAllDto<List<ProjetoUsuarioDto>>(itensPaginados, totalItens, itensPaginados.Count);
        }

        public async Task<ProjetoUsuarioDto> GetById(Guid idUsuario, Guid idProjeto) {
            var projetoUsuario = _uow.ProjetoUsuarioRepository.Find(x => !x.IsDeleted && x.IdUsuario == idUsuario && x.IdProjeto == idProjeto).FirstOrDefault();
            return _mapper.Map<ProjetoUsuarioDto>(projetoUsuario);
        }

        public async Task<ProjetoUsuarioDto> Upsert(ProjetoUsuarioDto projetoUsuarioDto, UserDto currentUser) {
            try {
                var projetoUsuario = _uow.ProjetoUsuarioRepository.Find(p => p.IdUsuario == projetoUsuarioDto.IdUsuario && p.IdProjeto == projetoUsuarioDto.IdProjeto).FirstOrDefault();
                bool insert = projetoUsuario == null;

                if (insert) {
                    projetoUsuario = new ProjetoUsuario {
                        IdUsuario = projetoUsuarioDto.IdUsuario,
                        IdProjeto = projetoUsuarioDto.IdProjeto
                    };
                }

                if (insert) {
                    projetoUsuario.Created = DateTime.UtcNow;
                    projetoUsuario.CreatedBy = new Guid(currentUser.Id);
                    projetoUsuario.IsDeleted = false;
                    _uow.ProjetoUsuarioRepository.Insert(projetoUsuario);
                }
                else {
                    projetoUsuario.Updated = DateTime.UtcNow;
                    projetoUsuario.UpdatedBy = new Guid(currentUser.Id);
                    _uow.ProjetoUsuarioRepository.Update(projetoUsuario);
                }

                await _uow.Save();
                return _mapper.Map<ProjetoUsuarioDto>(projetoUsuario);
            }
            catch {
                return null;
            }
        }

        public async Task<bool> Delete(Guid idUsuario, Guid idProjeto, UserDto currentUser) {
            var projetoUsuario = _uow.ProjetoUsuarioRepository.Find(x => x.IdUsuario == idUsuario && x.IdProjeto == idProjeto).FirstOrDefault();
            if (projetoUsuario != null) {
                projetoUsuario.Updated = DateTime.UtcNow;
                projetoUsuario.UpdatedBy = new Guid(currentUser.Id);
                projetoUsuario.IsDeleted = true;
                _uow.ProjetoUsuarioRepository.Update(projetoUsuario);
                await _uow.Save();
                return true;
            }
            return false;
        }
    }
}
