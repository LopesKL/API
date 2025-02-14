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

namespace Application.EmpresaHandler {
    public class EmpresaHandler {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly INotification _notification;

        public EmpresaHandler(IUnitOfWork uow, IMapper mapper, INotification notification, IConfiguration configuration) {
            _uow = uow;
            _mapper = mapper;
            _notification = notification;
            _configuration = configuration;
        }

        public async Task<ResponseAllDto<List<EmpresaDto>>> GetAll(RequestAllEmpresaDto request) {
            var consultaBase = _uow.EmpresaRepository.Find(x => !x.IsDeleted).AsQueryable();
            //consultaBase = consultaBase.ApplyFilters(request);

            if (!string.IsNullOrEmpty(request.SortOrder) && !string.IsNullOrEmpty(request.SorterField))
                consultaBase = consultaBase.ApplySorting(request.SorterField, request.SortOrder);
            else
                consultaBase = consultaBase.OrderByDescending(x => x.Updated).ThenByDescending(x => x.Created);

            var totalItens = await consultaBase.CountAsync();
            var itensPaginados = await consultaBase
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => _mapper.Map<EmpresaDto>(x))
                .ToListAsync();

            return new ResponseAllDto<List<EmpresaDto>>(itensPaginados, totalItens, itensPaginados.Count);
        }

        public async Task<EmpresaDto> GetById(Guid id) {
            var empresa = _uow.EmpresaRepository.Find(x => !x.IsDeleted && x.IdEmpresa == id).FirstOrDefault();
            return _mapper.Map<EmpresaDto>(empresa);
        }

        public async Task<EmpresaDto> Upsert(EmpresaDto empresaDto, UserDto currentUser) {
            try {
                var empresa = _uow.EmpresaRepository.Find(p => p.IdEmpresa == empresaDto.IdEmpresa).FirstOrDefault();
                bool insert = empresa == null;

                if (insert) {
                    empresa = new Empresa { IdEmpresa = Guid.NewGuid() };
                }

                empresa.Nome = empresaDto.Nome;

                if (insert) {
                    empresa.Created = DateTime.UtcNow;
                    //empresa.CreatedBy = new Guid(currentUser.Id);
                    empresa.IsDeleted = false;
                    _uow.EmpresaRepository.Insert(empresa);
                }
                else {
                    empresa.Updated = DateTime.UtcNow;
                    empresa.UpdatedBy = new Guid(currentUser.Id);
                    _uow.EmpresaRepository.Update(empresa);
                }

                await _uow.Save();
                return _mapper.Map<EmpresaDto>(empresa);
            }
            catch {
                return null;
            }
        }

        public async Task<bool> Delete(Guid id, UserDto currentUser) {
            var empresa = _uow.EmpresaRepository.Find(x => x.IdEmpresa == id).FirstOrDefault();
            if (empresa != null) {
                empresa.Updated = DateTime.UtcNow;
                empresa.UpdatedBy = new Guid(currentUser.Id);
                empresa.IsDeleted = true;
                _uow.EmpresaRepository.Update(empresa);
                await _uow.Save();
                return true;
            }
            return false;
        }
    }
}
