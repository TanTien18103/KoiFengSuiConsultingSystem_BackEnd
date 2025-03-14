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
        Task<List<BookingOffline>> GetBookingOfflinesByUserId(string userId);
        Task<BookingOffline> CreateBookingOffline(BookingOffline bookingOffline);
        Task<BookingOffline> UpdateBookingOffline(BookingOffline bookingOffline);
        Task DeleteBookingOffline(string bookingOfflineId);
    }
}
