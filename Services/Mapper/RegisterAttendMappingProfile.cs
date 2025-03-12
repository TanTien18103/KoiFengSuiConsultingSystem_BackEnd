using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.RegisterAttend;
using Services.ApiModels.Workshop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class RegisterAttendMappingProfile : Profile
    {
        public RegisterAttendMappingProfile()
        {
            CreateMap<RegisterAttend, RegisterAttendResponse>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Account.FullName))
                .ForMember(dest => dest.WorkshopName, opt => opt.MapFrom(src => src.Workshop.WorkshopName));
        }
    }
}
