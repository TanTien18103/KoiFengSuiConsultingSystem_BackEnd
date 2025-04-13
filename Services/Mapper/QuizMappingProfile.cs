using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.Course;
using Services.ApiModels.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class QuizMappingProfile : Profile
    {
        public QuizMappingProfile()
        {
            CreateMap<Quiz, QuizResponse>()
                .ForMember(dest => dest.MasterName, opt => opt.MapFrom(src => src.CreateByNavigation.MasterName))
                .ForMember(dest => dest.QuestionCount, opt => opt.MapFrom(src => src.Questions.Count))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.CourseName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Course.Description))
                .ForMember(dest => dest.MasterId, opt => opt.MapFrom(src => src.CreateBy));

            CreateMap<QuizRequest, Quiz>();
        }
    }
}
    