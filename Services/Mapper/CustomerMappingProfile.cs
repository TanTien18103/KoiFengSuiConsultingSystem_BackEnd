using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.Account;
using Services.ApiModels.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class CustomerMappingProfile : Profile
    {
        public CustomerMappingProfile()
        {
            CreateMap<Customer, ElementLifePalaceDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Account.FullName))
                .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.Account.Dob))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Account.Gender));

            CreateMap<Customer, CustomerResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Account.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Account.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Account.UserName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Account.PhoneNumber))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Account.FullName))
                .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.Account.Dob))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Account != null && src.Account.Gender.HasValue
                    ? (src.Account.Gender.Value ? "Male" : "Female") : "Unknown"))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Account.IsActive));
        }
    }
}
