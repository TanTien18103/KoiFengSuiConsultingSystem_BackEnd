using Services.ApiModels;
using Services.ApiModels.BookingOnline;

﻿using BusinessObjects.Enums;
using Services.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Services.Interfaces
{
    public interface IBookingOnlineService
    {
        Task<ResultModel> GetBookingOnlineById(string bookingId);
        Task<ResultModel> GetBookingOnlinesHover();
        Task<ResultModel> GetBookingByStatusAsync(BookingOnlineEnums? status, BookingTypeEnums? type);
        Task<ResultModel> AssignMasterToBooking(string bookingId, string masterId);
    }
}
