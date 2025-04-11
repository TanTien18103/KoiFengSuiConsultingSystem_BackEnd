using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.BookingOfflineRepository
{
    public interface IBookingOfflineRepo
    {
        Task<BookingOffline> GetBookingOfflineById(string bookingOfflineId);
        Task<BookingOffline> GetConsultingOfflineByMasterScheduleIdRepo(string masterScheduleId);
        Task<List<BookingOffline>> GetBookingOfflines();
        Task<List<BookingOffline>> GetBookingOfflinesByAccountId(string accountId);
        Task<List<BookingOffline>> GetBookingOfflinesByUserId(string userId);
        Task<BookingOffline> CreateBookingOffline(BookingOffline bookingOffline);
        Task<BookingOffline> UpdateBookingOffline(BookingOffline bookingOffline);
        Task<BookingOffline> UpdateBookingOfflineDocument(string bookingOfflineId, string documentId, string status);
        Task DeleteBookingOffline(string bookingOfflineId);
        Task<(BookingOffline booking, string message)> ProcessBookingTransaction(BookingOffline booking, string packageId, decimal selectedPrice);
        Task<List<BookingOffline>> GetBookingOfflinesByMasterIdRepo(string masterId);
        Task<List<BookingOffline>?> GetBookingsOfflineByCustomerId(string customerId);
        Task<List<BookingOffline>?> GetBookingsOfflineByStaffId(string staffId);
        Task<BookingOffline> GetPendingBookingByCustomerId(string customerId);
    }
}
