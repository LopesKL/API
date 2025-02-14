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

namespace Application.ClienteHandler {
    public class ClienteHandler {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly INotification _notification;

        public ClienteHandler(IUnitOfWork uow, IMapper mapper, INotification notification, IConfiguration configuration) {
            _uow = uow;
            _mapper = mapper;
            _notification = notification;
            _configuration = configuration;
        }

        public async Task<ResponseAllDto<List<ClienteDto>>> GetAll(RequestAllClienteDto request) {
            var consultaBase = _uow.ClienteRepository.Find(x => !x.IsDeleted).AsQueryable();

            //consultaBase = consultaBase.ApplyFilters(request);

            if (!string.IsNullOrEmpty(request.SortOrder) && !string.IsNullOrEmpty(request.SorterField))
                consultaBase = consultaBase.ApplySorting(request.SorterField, request.SortOrder);
            else
                consultaBase = consultaBase.OrderByDescending(x => x.Updated).ThenByDescending(x => x.Created);

            var totalItens = await consultaBase.CountAsync();
            var itensPaginados = await consultaBase
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => _mapper.Map<ClienteDto>(x))
                .ToListAsync();

            return new ResponseAllDto<List<ClienteDto>>(itensPaginados, totalItens, itensPaginados.Count);
        }

        public async Task<ClienteDto> GetById(Guid id) {
            var cliente = _uow.ClienteRepository.Find(x => !x.IsDeleted && x.IdCliente == id).FirstOrDefault();
            return _mapper.Map<ClienteDto>(cliente);
        }

        public async Task<ClienteDto> Upsert(ClienteDto clienteDto, UserDto currentUser) {
            try {
                var cliente = _uow.ClienteRepository.Find(p => p.IdCliente == clienteDto.IdCliente).FirstOrDefault();
                bool insert = cliente == null;

                if (insert) {
                    cliente = new Cliente { IdCliente = Guid.NewGuid() };
                }

                cliente.Nome = clienteDto.Nome;
                cliente.Moeda = clienteDto.Moeda;

                if (insert) {
                    cliente.Created = DateTime.UtcNow;
                    //cliente.CreatedBy = new Guid(currentUser.Id);
                    cliente.IsDeleted = false;
                    _uow.ClienteRepository.Insert(cliente);
                }
                else {
                    cliente.Updated = DateTime.UtcNow;
                    cliente.UpdatedBy = new Guid(currentUser.Id);
                    _uow.ClienteRepository.Update(cliente);
                }

                await _uow.Save();
                return _mapper.Map<ClienteDto>(cliente);
            }
            catch {
                return null;
            }
        }

        public async Task<bool> Delete(Guid id, UserDto currentUser) {
            var cliente = _uow.ClienteRepository.Find(x => x.IdCliente == id).FirstOrDefault();
            if (cliente != null) {
                cliente.Updated = DateTime.UtcNow;
                cliente.UpdatedBy = new Guid(currentUser.Id);
                cliente.IsDeleted = true;
                _uow.ClienteRepository.Update(cliente);
                await _uow.Save();
                return true;
            }
            return false;
        }
    }
}
