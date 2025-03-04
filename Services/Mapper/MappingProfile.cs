using AutoMapper;
using BusinessObjects.Models;
using KoiFengSuiConsultingSystem.Response;
using Services.Request;

namespace Services.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<KoiPond, KoiPondResponse>()
                .ForMember(dest => dest.ShapeName, opt => opt.MapFrom(src => src.Shape.ShapeName));

            CreateMap<Account, EditProfileRequest>();
        }
    }
} 