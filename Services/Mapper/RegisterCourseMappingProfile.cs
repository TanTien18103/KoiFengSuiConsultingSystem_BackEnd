using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.RegisterCourse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class RegisterCourseMappingProfile : Profile
    {
        public RegisterCourseMappingProfile() 
        { 
            CreateMap<RegisterCourse, RegisterCourseResponse>();
        }
    }
}
