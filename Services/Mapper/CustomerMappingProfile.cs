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
            CreateMap<Customer, ElementLifePalaceDto>();
        }
    }
}
