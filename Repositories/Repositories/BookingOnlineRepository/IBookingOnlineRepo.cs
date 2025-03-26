using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        Task<bool> CheckCustomerHasUncompletedBookingRepo(string customerId);
        Task<BookingOnline> UpdateBookingOnlineStatusRepo(string bookingOnlineId, string status);
        Task<BookingOnline> UpdateBookingOnlineMasterNoteRepo(string bookingOnlineId, string masterNote);
    }
}
