using AutoMapper;
using StellarBooks.Applications.DTOs;
using StellarBooks.Domain.Entities;

namespace StellarBooks.Application.MapperProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.Favorites, opt => opt.Ignore());

            CreateMap<User, UpdateUserDto>()
                .ForMember(dest => dest.Favorites, opt => opt.Ignore());

            CreateMap<CreateUserDto, User>();
            CreateMap<User, CreateUserDto>();

            CreateMap<UpdateTaleDto, Tale>()
                .ForMember(dest => dest.Favorites, opt => opt.Ignore())
                .ForMember(dest => dest.Activities, opt => opt.Ignore());

            CreateMap<Tale, UpdateTaleDto>();


            CreateMap<CreateTaleDto, Tale>();
            CreateMap<Tale, CreateTaleDto>();

            CreateMap<UpdateActivityDto, Activity>()
                .ForMember(dest => dest.Tale, opt => opt.Ignore());

            CreateMap<Activity, UpdateActivityDto>()
                .ForMember(dest => dest.Tale, opt => opt.Ignore());

            CreateMap<CreateActivityDto, Activity>();
            CreateMap<Activity, CreateActivityDto>();

            CreateMap<CreateFavoriteDto, Favorite>();
            CreateMap<Favorite, CreateFavoriteDto>();

            CreateMap<UpdateFavoriteDto, Favorite>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Tale, opt => opt.Ignore());

            CreateMap<Favorite, UpdateFavoriteDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Tale, opt => opt.MapFrom(src => src.Tale));

        }
    }
}
