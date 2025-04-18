﻿using AutoMapper;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Services.ApiModels.Attachment;
using Services.ApiModels.FengShuiDocument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class FengShuiDocumentMappingProfile : Profile
    {
        public FengShuiDocumentMappingProfile() 
        {
            CreateMap<FengShuiDocument, AllFengShuiDocumentResponse>();

            CreateMap<FengShuiDocument, FengShuiDocumentResponse>()
                .ForMember(dest => dest.DocumentId, opt => opt.MapFrom(src => src.FengShuiDocumentId))
                .ForMember(dest => dest.BookingOffline, opt => opt.MapFrom(src => src.BookingOfflines.FirstOrDefault()));
            CreateMap<BookingOffline, BookingOfflineInfo>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Account.FullName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => BookingOfflineEnums.DocumentConfirmedByManager.ToString()))
                .ForMember(dest => dest.MasterName, opt => opt.MapFrom(src => src.Master.MasterName));
        }
    }
}
