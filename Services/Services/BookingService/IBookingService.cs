using Services.ApiModels;
using BusinessObjects.Enums;
using Services.ApiModels.BookingOnline;

namespace Services.Services.BookingService
{
    public interface IBookingService
    {
        Task<ResultModel> GetBookingByIdAsync(string bookingId);
        Task<ResultModel> GetConsultingDetailByMasterScheduleIdAsync(string masterScheduleId);
        Task<ResultModel> GetBookingByStatusAsync(BookingOnlineEnums? status, BookingTypeEnums? type);
        Task<ResultModel> GetBookingOnlinesHoverAsync();
        Task<ResultModel> AssignMasterToBookingAsync(string bookingId, string masterId);
        Task<ResultModel> CreateBookingOnline(BookingOnlineRequest bookingOnlineRequest);
    }
}
