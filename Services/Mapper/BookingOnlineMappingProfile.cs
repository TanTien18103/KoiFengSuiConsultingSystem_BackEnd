using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.BookingOnline;
using Services.ApiModels.KoiVariety;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class BookingOnlineMappingProfile : Profile
    {
        public BookingOnlineMappingProfile() 
        {
            CreateMap<BookingOnline, BookingOnlineResponse>()
            .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer.Account.FullName))
            .ForMember(dest => dest.Master, opt => opt.MapFrom(src => src.Master.Account.FullName));
        }
    }
}
