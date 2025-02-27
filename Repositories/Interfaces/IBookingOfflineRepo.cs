using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IBookingOfflineRepo
    {
        Task<BookingOffline> GetBookingOfflineById(string bookingOfflineId);
        Task<List<BookingOffline>> GetBookingOfflinesByUserId(string userId);
        Task<BookingOffline> CreateBookingOffline(BookingOffline bookingOffline);
        Task<BookingOffline> UpdateBookingOffline(BookingOffline bookingOffline);
        Task DeleteBookingOffline(string bookingOfflineId);

    }
}
