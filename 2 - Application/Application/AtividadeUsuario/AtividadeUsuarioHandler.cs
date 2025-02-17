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
            var taskUsuario = _uow.AtividadeUsuarioRepository.Find(x => !x.IsDeleted && x.IdUsuario == idUsuario && x.IdAtividade == idAtividade).FirstOrDefault();
            return _mapper.Map<AtividadeUsuarioDto>(taskUsuario);
        }

        public async Task<AtividadeUsuarioDto> Upsert(AtividadeUsuarioDto atividadeUsuarioDto, UserDto currentUser) {
            try {
                var taskUsuario = _uow.AtividadeUsuarioRepository.Find(p => p.IdUsuario == atividadeUsuarioDto.IdUsuario && p.IdAtividade == atividadeUsuarioDto.IdAtividade).FirstOrDefault();
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

        //Essa função vamos usar na pagina inicial nos dropdown
        public async Task<AtividadeUsuarioDto> GetUserActivities(UserDto currentUser)
        {
            var today = DateTime.UtcNow;
            var nextWeek = today.AddDays(7);


            // Buscar todas as atividades do usuário e carregar a hierarquia completa
            var atividades = await _uow.AtividadeUsuarioRepository
                .Find(x => !x.IsDeleted && x.IdUsuario.ToString() == currentUser.Id)
                .Select(x => new
                {
                    ProjetoNome = x.Atividade.AtividadeFilho.AtividadePai.Projetos.Nome,
                    AtividadePaiNome = x.Atividade.AtividadeFilho.AtividadePai.Nome,
                    AtividadeFilhoNome = x.Atividade.AtividadeFilho.Nome,
                    AtividadeNome = x.Atividade.Nome,
                    x.Atividade.DataInicio,
                    x.Atividade.DataFim,
                    x.Atividade.Progresso
                })
                .ToListAsync();

            // Função para formatar a hierarquia corretamente
            string FormatarHierarquia(string projeto, string atividadePai, string atividadeFilho, string atividade)
            {
                return $"{projeto} → {atividadePai} → {atividadeFilho} → {atividade}";
            }

            // Separar as atividades conforme as datas
            var atividadesAtrasadas = atividades
                .Where(x => x.DataFim < today)
                .Select(x => new AtividadeDto
                {
                    NomeFormatado = FormatarHierarquia(x.ProjetoNome, x.AtividadePaiNome, x.AtividadeFilhoNome, x.AtividadeNome),
                    PorcentagemConclusao = x.Progresso,
                    TempoRestante = (x.DataFim - today).TotalHours
                })
                .ToList();

            var atividadesEmProgresso = atividades
                .Where(x => x.DataInicio <= today && x.DataFim >= today && x.Progresso > 0)
                .Select(x => new AtividadeDto
                {
                    NomeFormatado = FormatarHierarquia(x.ProjetoNome, x.AtividadePaiNome, x.AtividadeFilhoNome, x.AtividadeNome),
                    PorcentagemConclusao = x.Progresso,
                    TempoRestante = (x.DataFim - today).TotalHours
                })
                .ToList();

            var atividadesFuturas = atividades
                .Where(x => x.DataInicio > today && x.DataInicio <= nextWeek)
                .Select(x => new AtividadeDto
                {
                    NomeFormatado = FormatarHierarquia(x.ProjetoNome, x.AtividadePaiNome, x.AtividadeFilhoNome, x.AtividadeNome),
                    PorcentagemConclusao = x.Progresso,
                    TempoRestante = (x.DataFim - today).TotalHours
                })
                .ToList();

            // Criar e retornar o objeto diretamente
            return new AtividadeUsuarioDto
            {
                Atrasadas = atividadesAtrasadas,
                EmProgresso = atividadesEmProgresso,
                Futuras = atividadesFuturas
            };
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
