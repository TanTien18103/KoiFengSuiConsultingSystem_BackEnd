using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.Account;
using Services.ApiModels.KoiVariety;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class KoiVarietyMappingProfile : Profile
    {
        public KoiVarietyMappingProfile() 
        {
            CreateMap<KoiVariety, FishesWithColorsDTO>();

            CreateMap<KoiVariety, KoiVarietyDto>().ReverseMap();

            CreateMap<KoiVariety, KoiVarietyElementDTO>();

            CreateMap<KoiVariety, KoiVarietyResponse>()
            .ForMember(dest => dest.VarietyName, opt => opt.MapFrom(src => src.VarietyName))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.VarietyColors, opt => opt.MapFrom(src => src.VarietyColors));
            CreateMap<VarietyColor, VarietyColorResponse>();
            CreateMap<Color, ColorResponse>();
        }
    }
}
