using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.Chapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class ChapterMappingProfile : Profile
    {
        public ChapterMappingProfile()
        {
            CreateMap<Chapter, ChapterRespone>();
            CreateMap<ChapterRequest, Chapter>();
        }
    }
}
