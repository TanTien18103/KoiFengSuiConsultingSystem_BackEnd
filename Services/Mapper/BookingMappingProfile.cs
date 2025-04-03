using AutoMapper;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Services.ApiModels.Booking;
using Services.ApiModels.BookingOffline;
using Services.ApiModels.BookingOnline;
using Services.ApiModels.FengShuiDocument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapper
{
    public class BookingMappingProfile : Profile
    {
        public BookingMappingProfile()
        {
            // Booking Online
            CreateMap<BookingOnline, BookingResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.BookingOnlineId))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => BookingTypeEnums.Online.ToString()))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null && src.Customer.Account != null ? src.Customer.Account.FullName : null))
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Customer != null && src.Customer.Account != null ? src.Customer.Account.Email : null))
                .ForMember(dest => dest.MasterName, opt => opt.MapFrom(src => src.Master.MasterName));

            CreateMap<BookingOnlineRequest, BookingOnline>();

            CreateMap<BookingOnline, BookingOnlineHoverRespone>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Account.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Customer.Account.PhoneNumber))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "Online")); // Gán mặc định là "Online"

            CreateMap<BookingOnline, BookingOnlineDetailResponse>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null && src.Customer.Account != null ? src.Customer.Account.FullName : null))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => BookingTypeEnums.Online.ToString()))
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Customer != null && src.Customer.Account != null ? src.Customer.Account.Email : null))
                .ForMember(dest => dest.MasterName, opt => opt.MapFrom(src => src.Master != null && src.Master.Account != null ? src.Master.Account.FullName : null));

            CreateMap<BookingOnline, ConsultingOnlineDetailResponse>()
                .ForMember(dest => dest.ConsultingId, opt => opt.MapFrom(src => src.BookingOnlineId))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null && src.Customer.Account != null ? src.Customer.Account.FullName : null))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => BookingTypeEnums.Online.ToString()));


            // Booking Offline
            CreateMap<BookingOffline, ConsultingOfflineDetailResponse>()
                .ForMember(dest => dest.ConsultingId, opt => opt.MapFrom(src => src.BookingOfflineId))
                .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null && src.Customer.Account != null ? src.Customer.Account.FullName : null))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => BookingTypeEnums.Offline.ToString()));

            CreateMap<BookingOffline, BookingResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.BookingOfflineId))
                .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => BookingTypeEnums.Offline.ToString()))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null && src.Customer.Account != null ? src.Customer.Account.FullName : null))
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Customer != null && src.Customer.Account != null ? src.Customer.Account.Email : null))
                .ForMember(dest => dest.MasterName, opt => opt.MapFrom(src => src.Master.MasterName));
                //.ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.MasterSchedule != null ? src.MasterSchedule.Date : null));

            CreateMap<BookingOffline, BookingOfflineDetailResponse>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null && src.Customer.Account != null ? src.Customer.Account.FullName : null))
                .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => BookingTypeEnums.Offline.ToString()))
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Customer != null && src.Customer.Account != null ? src.Customer.Account.Email : null))
                .ForMember(dest => dest.MasterName, opt => opt.MapFrom(src => src.Master != null && src.Master.Account != null ? src.Master.Account.FullName : null));

            CreateMap<BookingOfflineRequest, BookingOffline>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<BookingOffline, BookingOfflineContractResponse>()
                .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => src.Contract));

            CreateMap<Contract, ContractUrlOnlyResponse>();
        }
    }
} 