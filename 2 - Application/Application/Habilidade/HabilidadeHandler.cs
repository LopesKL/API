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

namespace Application.HabilidadeHandler {
    public class HabilidadeHandler {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly INotification _notification;

        public HabilidadeHandler(IUnitOfWork uow, IMapper mapper, INotification notification, IConfiguration configuration) {
            _uow = uow;
            _mapper = mapper;
            _notification = notification;
            _configuration = configuration;
        }

        public async Task<ResponseAllDto<List<HabilidadeDto>>> GetAll(RequestAllHabilidadeDto request) {
            var consultaBase = _uow.HabilidadeRepository.Find(x => !x.IsDeleted).AsQueryable();
            //consultaBase = consultaBase.ApplyFilters(request);

            if (!string.IsNullOrEmpty(request.SortOrder) && !string.IsNullOrEmpty(request.SorterField))
                consultaBase = consultaBase.ApplySorting(request.SorterField, request.SortOrder);
            else
                consultaBase = consultaBase.OrderByDescending(x => x.Updated).ThenByDescending(x => x.Created);

            var totalItens = await consultaBase.CountAsync();
            var itensPaginados = await consultaBase
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => _mapper.Map<HabilidadeDto>(x))
                .ToListAsync();

            return new ResponseAllDto<List<HabilidadeDto>>(itensPaginados, totalItens, itensPaginados.Count);
        }

        public async Task<HabilidadeDto> GetById(Guid id) {
            var habilidade = _uow.HabilidadeRepository.Find(x => !x.IsDeleted && x.IdHabilidade == id).FirstOrDefault();
            return _mapper.Map<HabilidadeDto>(habilidade);
        }

        public async Task<HabilidadeDto> Upsert(HabilidadeDto habilidadeDto, UserDto currentUser) {
            try {
                var habilidade = _uow.HabilidadeRepository.Find(p => p.IdHabilidade == habilidadeDto.IdHabilidade).FirstOrDefault();
                bool insert = habilidade == null;

                if (insert) {
                    habilidade = new Habilidade { IdHabilidade = Guid.NewGuid() };
                }

                habilidade.Nome = habilidadeDto.Nome;

                if (insert) {
                    habilidade.Created = DateTime.UtcNow;
                    //habilidade.CreatedBy = new Guid(currentUser.Id);
                    habilidade.IsDeleted = false;
                    _uow.HabilidadeRepository.Insert(habilidade);
                }
                else {
                    habilidade.Updated = DateTime.UtcNow;
                    habilidade.UpdatedBy = new Guid(currentUser.Id);
                    _uow.HabilidadeRepository.Update(habilidade);
                }

                await _uow.Save();
                return _mapper.Map<HabilidadeDto>(habilidade);
            }
            catch {
                return null;
            }
        }

        public async Task<bool> Delete(Guid id, UserDto currentUser) {
            var habilidade = _uow.HabilidadeRepository.Find(x => x.IdHabilidade == id).FirstOrDefault();
            if (habilidade != null) {
                habilidade.Updated = DateTime.UtcNow;
                habilidade.UpdatedBy = new Guid(currentUser.Id);
                habilidade.IsDeleted = true;
                _uow.HabilidadeRepository.Update(habilidade);
                await _uow.Save();
                return true;
            }
            return false;
        }
    }
}
