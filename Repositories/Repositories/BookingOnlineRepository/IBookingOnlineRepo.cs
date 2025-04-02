using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DAOs.DAOs.BookingOnlineDAO;

namespace Repositories.Repositories.BookingOnlineRepository
{
    public interface IBookingOnlineRepo
    {
        Task<BookingOnline> GetBookingOnlineByIdRepo(string bookingOnlineId);
        Task<BookingOnline> GetConsultingOnlineByMasterScheduleIdRepo(string masterScheduleId);
        Task<List<BookingOnline>> GetBookingOnlinesRepo();
        Task<BookingOnline> CreateBookingOnlineRepo(BookingOnline bookingOnline);
        Task<BookingOnline> UpdateBookingOnlineRepo(BookingOnline bookingOnline);
        Task DeleteBookingOnlineRepo(string bookingOnlineId);
        Task<BookingCheckResult> CheckCustomerHasUncompletedBookingRepo(string customerId);
        Task<BookingOnline> UpdateBookingOnlineStatusRepo(string bookingOnlineId, string status);
        Task<BookingOnline> UpdateBookingOnlineMasterNoteRepo(string bookingOnlineId, string masterNote);
        Task<List<BookingOnline>> GetBookingOnlinesByMasterIdRepo(string masterId);
        Task<List<BookingOnline>> GetConflictingBookingsRepo(string masterId, DateOnly bookingDate, TimeOnly startTime);
        Task<List<BookingOnline>> GetUnpaidBookingsOlderThanRepo(DateTime cutoffDate);
        Task<BookingOnline> GetBookingOnlineByOrderIdRepo(string orderId);
        Task<BookingOnline> GetBookingOnlineByMasterScheduleIdRepo(string masterScheduleId);
        Task<List<BookingOnline>> GetBookingsByMasterAndTimeRepo(string masterId, TimeOnly startTime, TimeOnly endTime, DateOnly bookingDate);
        Task<BookingOnline> UpdateBookingOnlineWithTrackingRepo(BookingOnline bookingOnline);
        Task<List<BookingOnline>?> GetBookingsOnlineByCustomerId(string customerId);
        Task<List<BookingOnline>?> GetBookingsOnlineByStaffId(string staffId);

    }
}
