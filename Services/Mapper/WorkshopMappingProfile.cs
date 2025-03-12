using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.MasterSchedule;
using Services.ApiModels.Workshop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class WorkshopMappingProfile : Profile
    {
        public WorkshopMappingProfile()
        {
            CreateMap<WorkShop, WorkshopResponse>()
                .ForMember(dest => dest.MasterName, opt => opt.MapFrom(src => src.Master.MasterName));

            CreateMap<WorkshopRequest, WorkShop>()
                .ForMember(dest => dest.WorkshopId, opt => opt.Ignore());
        }
    }
}
