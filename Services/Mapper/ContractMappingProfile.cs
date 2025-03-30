using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class ContractMappingProfile : Profile
    {
        public ContractMappingProfile()
        {
            CreateMap<Contract, ContractResponse>()
                .ForMember(dest => dest.BookingOffline, opt => opt.MapFrom(src => src.BookingOfflines.FirstOrDefault()));
            CreateMap<BookingOffline, BookingOfflineInfoForContract>();
        }
    }
}
