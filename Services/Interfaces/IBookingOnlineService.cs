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
        Task<ResultModel<BookingOnlineDetailResponeDTO>> GetBookingOnlineById(string bookingId);
        Task<ResultModel<List<BookingOnlineHoverResponeDTO>>> GetBookingOnlines();

        Task<ResultModel> GetAllHistoryBookingOnlineAsync(BookingOnlineEnums? status);
        Task<ResultModel> ViewDetailsHistoryBookingOnlineAsync(string id);
    }
}
