using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.Account;
using Services.ApiModels.Color;
using Services.ApiModels.KoiVariety;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class ColorMappingProfile : Profile
    {
        public ColorMappingProfile() 
        {
            CreateMap<Color, ColorPercentageDto>();

            CreateMap<Color, ColorRequest>();
        }
    }
}
