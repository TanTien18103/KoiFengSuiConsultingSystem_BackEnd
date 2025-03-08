using BusinessObjects.Enums;
using Microsoft.AspNetCore.Http;
using Repositories.Interfaces;
using Services.ApiModels;
using Services.ApiModels.BookingOffline;
using Services.ApiModels.BookingOnline;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServicesHelpers
{
    public class BookingTypeService : IBookingTypeService
    {
        private readonly IBookingOnlineRepo _bookingOnlineRepo;
        private readonly IBookingOfflineRepo _bookingOfflineRepo;

        public BookingTypeService(IBookingOnlineRepo bookingOnlineRepository, IBookingOfflineRepo bookingOfflineRepository)
        {
            _bookingOnlineRepo = bookingOnlineRepository;
            _bookingOfflineRepo = bookingOfflineRepository;
        }

        public async Task<ResultModel> Calculate(BookingTypeEnums bookingType, string bookingId)
        {
            var res = new ResultModel();
            if (string.IsNullOrEmpty(bookingId))
            {
                res.IsSuccess = false;
                res.StatusCode = StatusCodes.Status400BadRequest;
                res.Message = "Invalid booking ID";
                return res;
            }

            switch (bookingType)
            {
                case BookingTypeEnums.Online:
                    var onlineBooking = await _bookingOnlineRepo.GetBookingOnlineByIdRepo(bookingId);
                    if (onlineBooking == null)
                    {
                        res.IsSuccess = false;
                        res.StatusCode = StatusCodes.Status404NotFound;
                        res.Message = "Online booking not found";
                        return res;
                    }

                    var onlineResponse = new BookingOnlineDetailRespone
                    {
                        BookingOnlineId = onlineBooking.BookingOnlineId,
                        Type = BookingTypeEnums.Online.ToString(),
                        Status = onlineBooking.Status,
                        CustomerName = onlineBooking.Customer.Account.FullName,
                        MasterName = onlineBooking.Master.Account.FullName,
                        Description = onlineBooking.Description,
                        BookingDate = onlineBooking.BookingDate,
                        StartTime = onlineBooking.StartTime,
                        EndTime = onlineBooking.EndTime,
                        MasterNote = onlineBooking.MasterNote
                    };

                    res.IsSuccess = true;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = onlineResponse;
                    res.Message = "Successfully retrieved online booking";
                    return res;

                case BookingTypeEnums.Offline:
                    var offlineBooking = await _bookingOfflineRepo.GetBookingOfflineById(bookingId);
                    if (offlineBooking == null)
                    {
                        res.IsSuccess = false;
                        res.StatusCode = StatusCodes.Status404NotFound;
                        res.Message = "Offline booking not found";
                        return res;
                    }

                    var offlineResponse = new BookingOfflineResponse
                    {
                        BookingOfflineId = offlineBooking.BookingOfflineId,
                        Type = BookingTypeEnums.Offline.ToString(),
                        Status = offlineBooking.Status,
                        CustomerName = offlineBooking.Customer.Account.FullName,
                        MasterName = offlineBooking.Master.Account.FullName,
                        Description = offlineBooking.Description,
                        Location = offlineBooking.Location
                    };

                    res.IsSuccess = true;
                    res.StatusCode = StatusCodes.Status200OK;
                    res.Data = offlineResponse;
                    res.Message = "Successfully retrieved offline booking";
                    return res;

                default:
                    res.IsSuccess = false;
                    res.StatusCode = StatusCodes.Status400BadRequest;
                    res.Message = "Invalid booking type";
                    return res;
            }
        }
    }
}
