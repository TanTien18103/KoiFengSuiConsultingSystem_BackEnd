using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.Booking;
using Services.ApiModels.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile() 
        {
            CreateMap<Order, OrderResponse>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Account.FullName));
        }
    }
}
