using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.BookingOnline;
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
            // Mapping cho BookingOnlineDetailResponeDTO
            CreateMap<BookingOnline, BookingOnlineDetailResponeDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Account.FullName));

            // Mapping cho BookingOnlineHoverResponeDTO
            CreateMap<BookingOnline, BookingOnlineHoverResponeDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Account.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Customer.Account.PhoneNumber))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "Online")); // Gán mặc định là "Online"
        }
    }
}
