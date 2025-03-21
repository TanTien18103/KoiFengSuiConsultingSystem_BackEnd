using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.Course;
using Services.ApiModels.Workshop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class CourseMappingProfile : Profile
    {
        public CourseMappingProfile()
        {
            CreateMap<Course, CourseRespone>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.CreateByNavigation.MasterName));

            CreateMap<CourseRequest, Course>()
                .ForMember(dest => dest.CourseId, opt => opt.Ignore());

            CreateMap<Course, CourseByMasterResponse>();
        }
    }
}
