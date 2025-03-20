using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.Booking;
using Services.ApiModels.ConsultationPackage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class ConsultationPackageMappingProfile : Profile
    {
        public ConsultationPackageMappingProfile() 
        {
            CreateMap<ConsultationPackage, ConsultationPackageResponse>();
        }
    }
}
