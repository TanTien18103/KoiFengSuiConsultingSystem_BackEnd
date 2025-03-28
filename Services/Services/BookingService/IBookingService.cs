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
        Task<ResultModel> CompleteBookingOnlineByMaster(string bookingOnlineId);
        Task<ResultModel> UpdateBookingOnlineMasterNote(string bookingOnlineId, UpdateMasterNoteRequest request);
        Task<ResultModel> GetBookingOnlinesByMaster();
        // Booking Offline
        Task<ResultModel> RemoveConsultationPackage(string id);
        Task<ResultModel> ProcessCompleteBooking(BookingOfflineRequest request, string packageId, decimal selectedPrice);
        Task<ResultModel> GetBookingOfflineForCurrentLogin();
        Task<ResultModel> GetBookingOfflinesByMaster();
        Task<ResultModel> CancelUnpaidBookings();
    }
}
