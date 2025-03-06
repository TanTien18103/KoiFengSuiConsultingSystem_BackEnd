using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.KoiPond;
using Services.ApiModels.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class MasterMappingProfile : Profile
    {
        public MasterMappingProfile()
        {
            CreateMap<Master, MasterListReponseDTO>();
            CreateMap<Master, MasterDetailReponseDTO>();
        }
    }
}
