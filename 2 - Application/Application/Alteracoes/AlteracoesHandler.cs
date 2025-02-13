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
using System.Text.Json;
using System.Threading.Tasks;
using INotification = API.Domain.Notifications.INotificationHandler;

namespace Application.AlteracoesHandler {
    public class AlteracoesHandler {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly INotification _notification;

        public AlteracoesHandler(IUnitOfWork uow, IMapper mapper, INotification notification, IConfiguration configuration) {
            _uow = uow;
            _mapper = mapper;
            _notification = notification;
            _configuration = configuration;
        }

        public async Task<ResponseAllDto<List<AlteracoesDto>>> GetAll(RequestAllAlteracoesDto request) {
            var consultaBase = _uow.AlteracoesRepository.Find(x => !x.IsDeleted).AsQueryable();
            consultaBase = consultaBase.ApplyFilters(request);

            if (!string.IsNullOrEmpty(request.SortOrder) && !string.IsNullOrEmpty(request.SorterField))
                consultaBase = consultaBase.ApplySorting(request.SorterField, request.SortOrder);
            else
                consultaBase = consultaBase.OrderByDescending(x => x.Updated).ThenByDescending(x => x.Created);

            var totalItens = await consultaBase.CountAsync();
            var itensPaginados = await consultaBase
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => _mapper.Map<AlteracoesDto>(x))
                .ToListAsync();

            return new ResponseAllDto<List<AlteracoesDto>>(itensPaginados, totalItens, itensPaginados.Count);
        }

        public async Task<AlteracoesDto> GetById(Guid id) {
            var alteracao = _uow.AlteracoesRepository.Find(x => !x.IsDeleted && x.IdAlteracao == id).FirstOrDefault();
            return _mapper.Map<AlteracoesDto>(alteracao);
        }

        public async Task<AlteracoesDto> Upsert(AlteracoesDto alteracoesDto, UserDto currentUser) {
            try {
                var alteracao = _uow.AlteracoesRepository.Find(p => p.IdAlteracao == alteracoesDto.IdAlteracao).FirstOrDefault();
                bool insert = alteracao == null;

                if (insert) {
                    alteracao = new Alteracoes { IdAlteracao = Guid.NewGuid() };
                }

                alteracao.IdUsuario = alteracoesDto.IdUsuario;
                alteracao.DataEHora = alteracoesDto.DataEHora;
                alteracao.NomeTabela = alteracoesDto.NomeTabela;
                alteracao.TipoOperacao = alteracoesDto.TipoOperacao;
                alteracao.ValoresAntigos = alteracoesDto.ValoresAntigos;
                alteracao.ValoresNovos = alteracoesDto.ValoresNovos;

                if (insert) {
                    alteracao.Created = DateTime.UtcNow;
                    alteracao.CreatedBy = new Guid(currentUser.Id);
                    alteracao.IsDeleted = false;
                    _uow.AlteracoesRepository.Insert(alteracao);
                }
                else {
                    alteracao.Updated = DateTime.UtcNow;
                    alteracao.UpdatedBy = new Guid(currentUser.Id);
                    _uow.AlteracoesRepository.Update(alteracao);
                }

                await _uow.Save();
                return _mapper.Map<AlteracoesDto>(alteracao);
            }
            catch {
                return null;
            }
        }

        public async Task<bool> Delete(Guid id, UserDto currentUser) {
            var alteracao = _uow.AlteracoesRepository.Find(x => x.IdAlteracao == id).FirstOrDefault();
            if (alteracao != null) {
                alteracao.Updated = DateTime.UtcNow;
                alteracao.UpdatedBy = new Guid(currentUser.Id);
                alteracao.IsDeleted = true;
                _uow.AlteracoesRepository.Update(alteracao);
                await _uow.Save();
                return true;
            }
            return false;
        }
    }
}
