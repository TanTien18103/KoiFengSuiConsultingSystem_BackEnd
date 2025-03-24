using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.Booking;
using Services.ApiModels.Shape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class ShapeMappingProfile : Profile
    {
        public ShapeMappingProfile() 
        {
            CreateMap<Shape, ShapResponse>();
        }
    }
}
