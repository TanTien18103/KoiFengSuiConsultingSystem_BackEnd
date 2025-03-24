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
            CreateMap<Course, CourseResponse>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.CreateByNavigation.MasterName))
                .ForMember(dest => dest.MasterId, opt => opt.MapFrom(src => src.CreateByNavigation.MasterId))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName));

            CreateMap<Course, CourseDetailResponse>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                .ForMember(dest => dest.MasterId, opt => opt.MapFrom(src => src.CreateByNavigation.MasterId))
                .ForMember(dest => dest.EnrolledStudents, opt => opt.Ignore())
                .ForMember(dest => dest.TotalChapters, opt => opt.Ignore())
                .ForMember(dest => dest.TotalQuestions, opt => opt.Ignore())
                .ForMember(dest => dest.TotalDuration, opt => opt.Ignore());

            CreateMap<CourseRequest, Course>()
                .ForMember(dest => dest.CourseId, opt => opt.Ignore());

            CreateMap<Course, CourseByMasterResponse>();
        }
    }
}
