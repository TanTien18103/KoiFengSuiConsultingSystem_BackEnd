using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.KoiPond;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class KoiPondMappingProfile : Profile
    {
        public KoiPondMappingProfile() 
        {
            CreateMap<KoiPond, KoiPondResponse>()
                .ForMember(dest => dest.ShapeName, opt => opt.MapFrom(src => src.Shape.ShapeName))
                .ForMember(dest => dest.Element, opt => opt.MapFrom(src => src.Shape.Element));

            CreateMap<KoiPondRequest, KoiPond>();
        }
    }
}
