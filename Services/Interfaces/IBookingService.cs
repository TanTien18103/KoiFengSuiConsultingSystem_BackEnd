using Services.ApiModels;
﻿using BusinessObjects.Enums;

namespace Services.Interfaces
{
    public interface IBookingService
    {
        Task<ResultModel> GetBookingByIdAsync(string bookingId);
        Task<ResultModel> GetBookingByStatusAsync(BookingOnlineEnums? status, BookingTypeEnums? type);
        Task<ResultModel> GetBookingOnlinesHoverAsync();
        Task<ResultModel> AssignMasterToBookingAsync(string bookingId, string masterId);
    }
}
