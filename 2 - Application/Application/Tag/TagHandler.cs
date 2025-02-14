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

namespace Application.TagHandler {
    public class TagHandler {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly INotification _notification;

        public TagHandler(IUnitOfWork uow, IMapper mapper, INotification notification, IConfiguration configuration) {
            _uow = uow;
            _mapper = mapper;
            _notification = notification;
            _configuration = configuration;
        }

        public async Task<ResponseAllDto<List<TagDto>>> GetAll(RequestAllTagDto request) {
            var consultaBase = _uow.TagRepository.Find(x => !x.IsDeleted).AsQueryable();
            //consultaBase = consultaBase.ApplyFilters(request);

            if (!string.IsNullOrEmpty(request.SortOrder) && !string.IsNullOrEmpty(request.SorterField))
                consultaBase = consultaBase.ApplySorting(request.SorterField, request.SortOrder);
            else
                consultaBase = consultaBase.OrderByDescending(x => x.Updated).ThenByDescending(x => x.Created);

            var totalItens = await consultaBase.CountAsync();
            var itensPaginados = await consultaBase
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => _mapper.Map<TagDto>(x))
                .ToListAsync();

            return new ResponseAllDto<List<TagDto>>(itensPaginados, totalItens, itensPaginados.Count);
        }

        public async Task<TagDto> GetById(Guid id) {
            var tag = _uow.TagRepository.Find(x => !x.IsDeleted && x.IdTag == id).FirstOrDefault();
            return _mapper.Map<TagDto>(tag);
        }

        public async Task<TagDto> Upsert(TagDto tagDto, UserDto currentUser) {
            try {
                var tag = _uow.TagRepository.Find(p => p.IdTag == tagDto.IdTag).FirstOrDefault();
                bool insert = tag == null;

                if (insert) {
                    tag = new Tag { IdTag = Guid.NewGuid() };
                }

                tag.Nome = tagDto.Nome;

                if (insert) {
                    tag.Created = DateTime.UtcNow;
                    //tag.CreatedBy = new Guid(currentUser.Id);
                    tag.IsDeleted = false;
                    _uow.TagRepository.Insert(tag);
                }
                else {
                    tag.Updated = DateTime.UtcNow;
                    tag.UpdatedBy = new Guid(currentUser.Id);
                    _uow.TagRepository.Update(tag);
                }

                await _uow.Save();
                return _mapper.Map<TagDto>(tag);
            }
            catch {
                return null;
            }
        }

        public async Task<bool> Delete(Guid id, UserDto currentUser) {
            var tag = _uow.TagRepository.Find(x => x.IdTag == id).FirstOrDefault();
            if (tag != null) {
                tag.Updated = DateTime.UtcNow;
                tag.UpdatedBy = new Guid(currentUser.Id);
                tag.IsDeleted = true;
                _uow.TagRepository.Update(tag);
                await _uow.Save();
                return true;
            }
            return false;
        }
    }
}
