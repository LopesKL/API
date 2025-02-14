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

namespace Application.LancamentoHandler {
    public class LancamentoHandler {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly INotification _notification;

        public LancamentoHandler(IUnitOfWork uow, IMapper mapper, INotification notification, IConfiguration configuration) {
            _uow = uow;
            _mapper = mapper;
            _notification = notification;
            _configuration = configuration;
        }

        public async Task<ResponseAllDto<List<LancamentoDto>>> GetAll(RequestAllLancamentoDto request) {
            var consultaBase = _uow.LancamentoRepository.Find(x => !x.IsDeleted).AsQueryable();
            consultaBase = consultaBase.ApplyFilters(request);

            if (!string.IsNullOrEmpty(request.SortOrder) && !string.IsNullOrEmpty(request.SorterField))
                consultaBase = consultaBase.ApplySorting(request.SorterField, request.SortOrder);
            else
                consultaBase = consultaBase.OrderByDescending(x => x.Updated).ThenByDescending(x => x.Created);

            var totalItens = await consultaBase.CountAsync();
            var itensPaginados = await consultaBase
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => _mapper.Map<LancamentoDto>(x))
                .ToListAsync();

            return new ResponseAllDto<List<LancamentoDto>>(itensPaginados, totalItens, itensPaginados.Count);
        }

        public async Task<LancamentoDto> GetById(Guid id) {
            var lancamento = _uow.LancamentoRepository.Find(x => !x.IsDeleted && x.IdLancamento == id).FirstOrDefault();
            return _mapper.Map<LancamentoDto>(lancamento);
        }

        public async Task<LancamentoDto> Upsert(LancamentoDto lancamentoDto, UserDto currentUser) {
            try {
                var lancamento = _uow.LancamentoRepository.Find(p => p.IdLancamento == lancamentoDto.IdLancamento).FirstOrDefault();
                bool insert = lancamento == null;

                if (insert) {
                    lancamento = new Lancamento { IdLancamento = Guid.NewGuid() };
                }

                lancamento.IdProjeto = lancamentoDto.IdProjeto;
                lancamento.IdAtividadePai = lancamentoDto.IdAtividadePai;
                lancamento.IdAtividadeFilho = lancamentoDto.IdAtividadeFilho;
                lancamento.idTag = lancamentoDto.idTag;
                lancamento.IdUsuario = lancamentoDto.IdUsuario;
                lancamento.Descricao = lancamentoDto.Descricao;
                lancamento.Data = lancamentoDto.Data;
                lancamento.HorarioInicio = lancamentoDto.HorarioInicio;
                lancamento.HorarioFim = lancamentoDto.HorarioFim;
                lancamento.Horas = lancamentoDto.Horas;
                lancamento.Cobrado = lancamentoDto.Cobrado;
                lancamento.Valor = lancamentoDto.Valor;

                if (insert) {
                    lancamento.Created = DateTime.UtcNow;
                    //lancamento.CreatedBy = new Guid(currentUser.Id);
                    lancamento.IsDeleted = false;
                    _uow.LancamentoRepository.Insert(lancamento);
                }
                else {
                    lancamento.Updated = DateTime.UtcNow;
                    lancamento.UpdatedBy = new Guid(currentUser.Id);
                    _uow.LancamentoRepository.Update(lancamento);
                }

                await _uow.Save();
                return _mapper.Map<LancamentoDto>(lancamento);
            }
            catch {
                return null;
            }
        }

        public async Task<bool> Delete(Guid id, UserDto currentUser) {
            var lancamento = _uow.LancamentoRepository.Find(x => x.IdLancamento == id).FirstOrDefault();
            if (lancamento != null) {
                lancamento.Updated = DateTime.UtcNow;
                lancamento.UpdatedBy = new Guid(currentUser.Id);
                lancamento.IsDeleted = true;
                _uow.LancamentoRepository.Update(lancamento);
                await _uow.Save();
                return true;
            }
            return false;
        }
    }
}
