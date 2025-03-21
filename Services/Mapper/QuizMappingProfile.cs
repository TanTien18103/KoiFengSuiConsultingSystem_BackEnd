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
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course.CourseName));

            CreateMap<QuizRequest, Quiz>();
        }
    }
}
