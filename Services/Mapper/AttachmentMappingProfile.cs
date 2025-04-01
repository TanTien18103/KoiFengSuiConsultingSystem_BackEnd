using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.Answer;
using Services.ApiModels.Attachment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class AttachmentMappingProfile : Profile
    {
        public AttachmentMappingProfile() 
        {
            CreateMap<Attachment, AllAttachmentResponse>();

            CreateMap<Attachment, AttachmentResponse>()
                .ForMember(dest => dest.BookingOffline, opt => opt.MapFrom(src => src.BookingOfflines.FirstOrDefault()));
            CreateMap<BookingOffline, BookingOfflineInfoForAttachment>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Account.FullName))
                .ForMember(dest => dest.MasterName, opt => opt.MapFrom(src => src.Master.MasterName));
        }
    }
}
