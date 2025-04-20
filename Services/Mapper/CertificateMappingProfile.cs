using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.Certificate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class CertificateMappingProfile : Profile
    {
        public CertificateMappingProfile()
        {
            CreateMap<Certificate, CertificateResponse>()
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.Courses.FirstOrDefault() != null ? src.Courses.FirstOrDefault().CourseId : null))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Courses.FirstOrDefault() != null ? src.Courses.FirstOrDefault().CourseName : null));

            CreateMap<CertificateRequest, Certificate>();
        }
    }
}
