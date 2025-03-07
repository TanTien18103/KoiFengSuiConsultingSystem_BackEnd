using Services.ApiModels;
using Services.ApiModels.BookingOnline;

﻿using BusinessObjects.Enums;
using Services.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IBookingOnlineService
    {
        Task<ResultModel> GetBookingOnlineById(string bookingId);
        Task<ResultModel> GetBookingOnlines();

        Task<ResultModel> GetBookingOnlineByStatusAsync(BookingOnlineEnums? status);
    }
}
