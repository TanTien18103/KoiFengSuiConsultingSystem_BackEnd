using Services.ApiModels;
using BusinessObjects.Enums;
using Services.ApiModels.BookingOnline;
using Services.ApiModels.BookingOffline;

namespace Services.Services.BookingService
{
    public interface IBookingService
    {
        // Booking Online
        Task<ResultModel> GetBookingByIdAsync(string bookingId);
        Task<ResultModel> GetConsultingDetailByMasterScheduleIdAsync(string masterScheduleId);
        Task<ResultModel> GetBookingByStatusAsync(BookingOnlineEnums? status, BookingTypeEnums? type);
        Task<ResultModel> GetBookingOnlinesHoverAsync();
        Task<ResultModel> AssignMasterToBookingAsync(string? bookingonlineId, string? bookingofflineId, string masterId);
        Task<ResultModel> CreateBookingOnline(BookingOnlineRequest bookingOnlineRequest);

        // Booking Offline
        Task<ResultModel> CreateBookingOffline(BookingOfflineRequest request);
        Task<ResultModel> AddConsultationPackage(string packageId, string id);
        Task<ResultModel> RemoveConsultationPackage(string id);
        Task<ResultModel> SelectBookingOfflinePrice(string bookingId, decimal selectedPrice);
    }
}
