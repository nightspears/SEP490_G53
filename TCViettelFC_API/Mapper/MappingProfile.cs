using TCViettelFC_API.Dtos;
using TCViettelFC_API.Models;
using AutoMapper;
namespace TCViettelFC_API.Mapper
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName))
                .ReverseMap();
            CreateMap<UserCreateDto, User>().ReverseMap();
        }
    }
}
