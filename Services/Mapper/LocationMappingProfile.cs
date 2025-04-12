using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.KoiVariety;
using Services.ApiModels.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class LocationMappingProfile : Profile
    {
        public LocationMappingProfile()
        {
            CreateMap<LocationRequest, Location>();

            CreateMap<LocationUpdateRequest, Location>();

            CreateMap<Location, LocationResponse>();
        }
    }
}
