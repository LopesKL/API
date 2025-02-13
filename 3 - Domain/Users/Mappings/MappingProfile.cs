using AutoMapper;
using API.Application.Dto;
using API.Domain.Users.Auth;

namespace API.Domain.Mapper
{
    public class ApplicationMappingProfile : Profile
    {
        public ApplicationMappingProfile()
        {
            CreateMap<AppUser, UserDto>();
        }
    }
}
