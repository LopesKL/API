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

namespace Application.AtividadeUsuarioHandler {
    public class AtividadeUsuarioHandler {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly INotification _notification;

        public AtividadeUsuarioHandler(IUnitOfWork uow, IMapper mapper, INotification notification, IConfiguration configuration) {
            _uow = uow;
            _mapper = mapper;
            _notification = notification;
            _configuration = configuration;
        }

        public async Task<ResponseAllDto<List<AtividadeUsuarioDto>>> GetAll(RequestAllAtividadeUsuarioDto request) {
            var consultaBase = _uow.AtividadeUsuarioRepository.Find(x => !x.IsDeleted).AsQueryable();
            consultaBase = consultaBase.ApplyFilters(request);

            if (!string.IsNullOrEmpty(request.SortOrder) && !string.IsNullOrEmpty(request.SorterField))
                consultaBase = consultaBase.ApplySorting(request.SorterField, request.SortOrder);
            else
                consultaBase = consultaBase.OrderByDescending(x => x.Updated).ThenByDescending(x => x.Created);

            var totalItens = await consultaBase.CountAsync();
            var itensPaginados = await consultaBase
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => _mapper.Map<AtividadeUsuarioDto>(x))
                .ToListAsync();

            return new ResponseAllDto<List<AtividadeUsuarioDto>>(itensPaginados, totalItens, itensPaginados.Count);
        }

        public async Task<AtividadeUsuarioDto> GetById(Guid idUsuario, Guid idAtividade) {
            var taskUsuario = _uow.AtividadeUsuarioRepository.Find(x => !x.IsDeleted && x.IdUsuario == idUsuario && x.IdUsuario == idAtividade).FirstOrDefault();
            return _mapper.Map<AtividadeUsuarioDto>(taskUsuario);
        }

        public async Task<AtividadeUsuarioDto> Upsert(AtividadeUsuarioDto atividadeUsuarioDto, UserDto currentUser) {
            try {
                var taskUsuario = _uow.AtividadeUsuarioRepository.Find(p => p.IdUsuario == atividadeUsuarioDto.IdUsuario && p.IdUsuario == atividadeUsuarioDto.IdUsuario).FirstOrDefault();
                bool insert = taskUsuario == null;

                if (insert) {
                    taskUsuario = new AtividadeUsuario {
                        IdUsuario = atividadeUsuarioDto.IdUsuario,
                        IdAtividade = atividadeUsuarioDto.IdAtividade,
                    };
                }

                if (insert) {
                    taskUsuario.Created = DateTime.UtcNow;
                    taskUsuario.CreatedBy = new Guid(currentUser.Id);
                    taskUsuario.IsDeleted = false;
                    _uow.AtividadeUsuarioRepository.Insert(taskUsuario);
                }
                else {
                    taskUsuario.Updated = DateTime.UtcNow;
                    taskUsuario.UpdatedBy = new Guid(currentUser.Id);
                    _uow.AtividadeUsuarioRepository.Update(taskUsuario);
                }

                await _uow.Save();
                return _mapper.Map<AtividadeUsuarioDto>(taskUsuario);
            }
            catch {
                return null;
            }
        }

        //public async Task<bool> Delete(Guid idUsuario, Guid idAtividade) {
        //    var taskUsuario = _uow.AtividadeUsuarioRepository
        //        .Find(x => x.IdUsuario == idUsuario && x.IdUsuario == idAtividade)
        //        .FirstOrDefault();

        //    if (taskUsuario != null) {
        //        taskUsuario.Updated = DateTime.UtcNow;
        //        // Se necessário, defina UpdatedBy ou remova, conforme a lógica do negócio.
        //        // taskUsuario.UpdatedBy = Guid.Empty; // Exemplo
        //        taskUsuario.IsDeleted = true;
        //        _uow.AtividadeUsuarioRepository.Update(taskUsuario);
        //        await _uow.Save();
        //        return true;
        //    }

        //    return false;
        //}

    }
}
