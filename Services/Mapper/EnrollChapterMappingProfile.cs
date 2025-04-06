using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.Booking;
using Services.ApiModels.EnrollChapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class EnrollChapterMappingProfile : Profile
    {
        public EnrollChapterMappingProfile()
        {
            CreateMap<EnrollChapter, EnrollChapterResponse>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.EnrollCourse.CustomerId))
                .ForMember(dest => dest.ChapterName, opt => opt.MapFrom(src => src.Chapter.Title));
        }
    }
}
