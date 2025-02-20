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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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

        public async Task<LancamentoDto> Upsert(LancamentoDto lancamentoDto, UserDto currentUser)
        {
            try
            {
                // Busca um lançamento existente para a mesma atividade e data
                var lancamento = _uow.LancamentoRepository
                    .Find(p => p.IdAtividade == lancamentoDto.IdAtividade && p.Data.Date == lancamentoDto.Dia.Date)
                    .FirstOrDefault();

                bool insert = lancamento == null;

                // Converte a string de horas para decimal, tratando casos inválidos
                if (!decimal.TryParse(lancamentoDto.Horas, NumberStyles.Any, CultureInfo.InvariantCulture, out var horasDecimal) || horasDecimal < 0)
                {
                    _notification.DefaultBuilder()
                        .Code("Upsert_lancamento_05")
                        .Message("Horas informadas são inválidas.")
                        .RaiseNotification();
                    return null;
                }

                if (insert)
                {
                    lancamento = new Lancamento
                    {
                        IdLancamento = Guid.NewGuid(),
                        Data = lancamentoDto.Dia.Date,
                        Horas = horasDecimal // Substitui TimeSpan por decimal
                    };
                }
                else
                {
                    // Soma as horas ao lançamento existente
                    lancamento.Horas = horasDecimal;
                }

                // Busca e valida a atividade
                var atividade = _uow.AtividadeRepository.Find(x => x.IdAtividade == lancamentoDto.IdAtividade).FirstOrDefault();
                if (atividade == null)
                {
                    _notification.DefaultBuilder()
                        .Code("Upsert_lancamento_04")
                        .Message("Atividade está nula.")
                        .RaiseNotification();
                    return null;
                }

                // Busca e valida hierarquia de atividades
                var atividadeFilho = _uow.AtividadeFilhoRepository.Find(x => x.IdAtividadeFilho == atividade.IdAtividadeFilho).FirstOrDefault();
                if (atividadeFilho == null)
                {
                    _notification.DefaultBuilder()
                        .Code("Upsert_lancamento_01")
                        .Message("Atividade filho está nula.")
                        .RaiseNotification();
                    return null;
                }

                var atividadePai = _uow.AtividadePaiRepository.Find(x => x.IdAtividadePai == atividadeFilho.IdAtividadePai).FirstOrDefault();
                if (atividadePai == null)
                {
                    _notification.DefaultBuilder()
                        .Code("Upsert_lancamento_02")
                        .Message("Atividade pai está nula.")
                        .RaiseNotification();
                    return null;
                }

                var projeto = _uow.ProjetosRepository.Find(x => x.IdProjetos == atividadePai.IdProjeto).FirstOrDefault();
                if (projeto == null)
                {
                    _notification.DefaultBuilder()
                        .Code("Upsert_lancamento_03")
                        .Message("Projeto está nulo.")
                        .RaiseNotification();
                    return null;
                }

                // Atribui os relacionamentos e informações ao lançamento
                lancamento.IdProjeto = projeto.IdProjetos;
                lancamento.IdAtividadePai = atividadePai.IdAtividadePai;
                lancamento.IdAtividadeFilho = atividadeFilho.IdAtividadeFilho;
                lancamento.IdAtividade = atividade.IdAtividade;
                lancamento.idTag = lancamentoDto.idTag;
                lancamento.IdUsuario = new Guid(currentUser.Id);
                if (lancamentoDto.Horas == "0")
                {
                    lancamento.Descricao = "";
                }
                else
                {
                    lancamento.Descricao = lancamentoDto.Descricao;
                }
                lancamento.HorarioInicio = lancamentoDto.HorarioInicio;
                lancamento.HorarioFim = lancamentoDto.HorarioFim;
                lancamento.Cobrado = true;

                // Calcula o valor com base na taxa de faturamento
                lancamento.Valor = currentUser.TaxaFaturamento * horasDecimal;

                if (insert)
                {
                    lancamento.Created = DateTime.UtcNow;
                    lancamento.CreatedBy = new Guid(currentUser.Id);
                    lancamento.IsDeleted = false;
                    _uow.LancamentoRepository.Insert(lancamento);
                }
                else
                {
                    lancamento.Updated = DateTime.UtcNow;
                    lancamento.UpdatedBy = new Guid(currentUser.Id);
                    _uow.LancamentoRepository.Update(lancamento);
                }

                await _uow.Save();
                return _mapper.Map<LancamentoDto>(lancamento);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no Upsert: {ex.Message}");
                return null;
            }
        }



        public async Task<LancamentoDto> GetByLancamento(Guid idAtividade, DateTime dia)
        {
            var lancamentos = await _uow.LancamentoRepository
                .Find(l => l.IdAtividade == idAtividade && l.Data.Date == dia.Date && !l.IsDeleted)
                .ToListAsync();

            if (lancamentos.Any())
            {
                // Soma as horas convertendo para decimal e tratando valores inválidos
                decimal totalHoras = lancamentos.Sum(l =>
                {
                    if (l.Horas != null && decimal.TryParse(l.Horas.ToString(), out var horas) && horas >= 0)
                    {
                        return horas;
                    }
                    return 0;
                });

                return new LancamentoDto
                {
                    IdAtividade = idAtividade,
                    Data = dia,
                    Horas = totalHoras.ToString("F2", System.Globalization.CultureInfo.InvariantCulture), // Formata com duas casas decimais e separador decimal correto
                    Descricao = lancamentos.First().Descricao ?? string.Empty // Usa a primeira descrição ou vazio se nulo
                };
            }

            // Se não houver lançamentos, retorna "0.00" e descrição vazia
            return new LancamentoDto
            {
                IdAtividade = idAtividade,
                Data = dia,
                Horas = "0.00",
                Descricao = string.Empty
            };
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
