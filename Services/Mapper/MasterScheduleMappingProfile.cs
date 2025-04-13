using AutoMapper;
using BusinessObjects.Models;
using Services.ApiModels.Customer;
using Services.ApiModels.MasterSchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Services.ApiModels.MasterSchedule.MasterScheduleDTO;

namespace Services.Mapper
{
    public class MasterScheduleMappingProfile : Profile
    {
        public MasterScheduleMappingProfile()
        {
            CreateMap<MasterSchedule, MasterScheduleDTO>()
                .ForMember(dest => dest.MasterScheduleId, opt => opt.MapFrom(src => src.MasterScheduleId))
                .ForMember(dest => dest.MasterId, opt => opt.MapFrom(src => src.MasterId))
                .ForMember(dest => dest.MasterName, opt => opt.MapFrom(src => src.Master != null ? src.Master.MasterName : null))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
                .ForMember(dest => dest.BookingOnlines, opt => opt.MapFrom(src => src.BookingOnlines));

            CreateMap<MasterSchedule, MasterSchedulesListDTO>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.Schedules, opt => opt.MapFrom(src => new List<MasterScheduleDTO> {
                new MasterScheduleDTO
                {
                    MasterScheduleId = src.MasterScheduleId,
                    MasterId = src.MasterId,
                    MasterName = src.Master.MasterName,
                    StartTime = src.StartTime,
                    EndTime = src.EndTime,
                    BookingOnlines = src.BookingOnlines.Select(b => new MasterScheduleDTO.BookingOnlineDTO
                    {
                        Customer = new MasterScheduleDTO.CustomerInfoDTO
                        {
                            CustomerId = b.Customer.CustomerId,
                            FullName = b.Customer.Account.FullName,
                            Email = b.Customer.Account.Email,
                            PhoneNumber = b.Customer.Account.PhoneNumber
                        }
                    }).ToList()
                }
                }));

            CreateMap<BookingOnline, BookingOnlineDTO>()
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer));

            CreateMap<Customer, CustomerInfoDTO>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Account.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Account.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Account.PhoneNumber));

            CreateMap<MasterSchedule, MasterSchedulesForMobileResponse>();
        }
    }

}
