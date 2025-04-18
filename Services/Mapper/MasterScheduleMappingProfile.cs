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
                .ForMember(dest => dest.BookingOnlines, opt => opt.MapFrom(src => src.BookingOnlines))
                .ForMember(dest => dest.BookingOfflines, opt => opt.MapFrom(src => src.BookingOfflines))
                .ForMember(dest => dest.Workshops, opt => opt.MapFrom(src => src.WorkShops));

            CreateMap<MasterSchedule, MasterSchedulesListDTO>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.Schedules, opt => opt.MapFrom(src => new List<MasterScheduleDTO> {
                    new MasterScheduleDTO
                    {
                        MasterScheduleId = src.MasterScheduleId,
                        MasterId = src.MasterId,
                        MasterName = src.Master != null ? src.Master.MasterName : null,
                        StartTime = src.StartTime,
                        EndTime = src.EndTime,
                        BookingOnlines = src.BookingOnlines.Select(b => new BookingOnlineDTO
                        {
                            BookingOnlineId = b.BookingOnlineId,
                            Customer = new CustomerInfoDTO
                            {
                                CustomerId = b.Customer.CustomerId,
                                FullName = b.Customer.Account.FullName,
                                Email = b.Customer.Account.Email,
                                PhoneNumber = b.Customer.Account.PhoneNumber
                            }
                        }).ToList(),
                        BookingOfflines = src.BookingOfflines.Select(b => new BookingOfflineDTO
                        {
                            BookingOfflineId = b.BookingOfflineId,
                            Customer = new CustomerInfoDTO
                            {
                                CustomerId = b.Customer.CustomerId,
                                FullName = b.Customer.Account.FullName,
                                Email = b.Customer.Account.Email,
                                PhoneNumber = b.Customer.Account.PhoneNumber
                            },
                            Location = b.Location
                        }).ToList(),
                        Workshops = src.WorkShops.Select(w => new WorkshopDTO
                        {
                            WorkshopId = w.WorkshopId,
                            WorkshopName = w.WorkshopName,
                            LocationId = w.LocationId,
                            LocationName = w.Location != null ? w.Location.LocationName : null
                        }).ToList()
                    }
                }));

            CreateMap<BookingOnline, BookingOnlineDTO>()
                .ForMember(dest => dest.BookingOnlineId, opt => opt.MapFrom(src => src.BookingOnlineId))
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer));

            CreateMap<BookingOffline, BookingOfflineDTO>()
            .ForMember(dest => dest.BookingOfflineId, opt => opt.MapFrom(src => src.BookingOfflineId))
            .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location));

            CreateMap<WorkShop, WorkshopDTO>()
                .ForMember(dest => dest.WorkshopId, opt => opt.MapFrom(src => src.WorkshopId))
                .ForMember(dest => dest.WorkshopName, opt => opt.MapFrom(src => src.WorkshopName))
                .ForMember(dest => dest.LocationId, opt => opt.MapFrom(src => src.LocationId))
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.Location != null ? src.Location.LocationName : null)); 

            CreateMap<Customer, CustomerInfoDTO>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Account.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Account.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Account.PhoneNumber));

            CreateMap<MasterSchedule, MasterSchedulesForMobileResponse>();
        }
    }

}
