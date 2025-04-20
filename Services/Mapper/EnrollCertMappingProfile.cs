using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.EnrollCert;
using Services.ApiModels.EnrollChapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class EnrollCertMappingProfile : Profile
    {
        public EnrollCertMappingProfile()
        {
            CreateMap<EnrollCert, EnrollCertificateCurrentCustomerResponse>()
                .ForMember(dest => dest.EnrollCertId, opt => opt.MapFrom(src => src.EnrollCertId))
                .ForMember(dest => dest.CertificateId, opt => opt.MapFrom(src => src.CertificateId))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.CertificateImageUrl, opt => opt.MapFrom(src => src.Certificate.CertificateImage))
                .ForMember(dest => dest.RegisterCourseId, opt => opt.MapFrom(src => src.RegisterCourses.FirstOrDefault().EnrollCourseId))
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.RegisterCourses.FirstOrDefault().CourseId))
                .ForMember(dest => dest.Point, opt => opt.MapFrom(src => src.RegisterCourses.FirstOrDefault().EnrollQuiz.Point ?? 0))
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.RegisterCourses.FirstOrDefault().Course.CourseName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.RegisterCourses.FirstOrDefault().Course.Description))
                .ForMember(dest => dest.Introduction, opt => opt.MapFrom(src => src.RegisterCourses.FirstOrDefault().Course.Introduction))
                .ForMember(dest => dest.CourseImageUrl, opt => opt.MapFrom(src => src.RegisterCourses.FirstOrDefault().Course.ImageUrl))
                .ForMember(dest => dest.MasterName, opt => opt.MapFrom(src => src.RegisterCourses.FirstOrDefault().Course.CreateByNavigation.MasterName));
        }
    }
}