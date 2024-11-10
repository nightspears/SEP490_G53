using TCViettelFC_API.Dtos;
using TCViettelFC_API.Models;
using AutoMapper;
using TCViettelFC_API.Dtos.Order;
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

            CreateMap<Feedback, FeedbackDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Responder != null ? src.Responder.UserId : 0))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Creator != null ? src.Creator.CustomerId : 0))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Creator != null ? src.Creator.Email : string.Empty))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Creator != null ? src.Creator.Phone : string.Empty))
                .ReverseMap() // This enables reverse mapping from FeedbackDto to Feedback
                .ForMember(dest => dest.ResponderId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.CreatorId, opt => opt.MapFrom(src => src.CustomerId));
        }
    }
    }

