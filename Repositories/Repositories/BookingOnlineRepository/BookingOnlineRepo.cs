using BusinessObjects.Models;
using DAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.BookingOnlineRepository
{
    public class BookingOnlineRepo : IBookingOnlineRepo
    {
        public Task<BookingOnline> GetBookingOnlineByIdRepo(string bookingOnlineId)
        {
            return BookingOnlineDAO.Instance.GetBookingOnlineByIdDao(bookingOnlineId);
        }
        public Task<BookingOnline> GetConsultingOnlineByMasterScheduleIdRepo(string masterScheduleId)
        {
            return BookingOnlineDAO.Instance.GetConsultingOnlineByMasterScheduleIdDao(masterScheduleId);
        }
        public Task<List<BookingOnline>> GetBookingOnlinesRepo()
        {
            return BookingOnlineDAO.Instance.GetBookingOnlinesDao();
        }
        public Task<BookingOnline> CreateBookingOnlineRepo(BookingOnline bookingOnline)
        {
            return BookingOnlineDAO.Instance.CreateBookingOnlineDao(bookingOnline);
        }
        public Task<BookingOnline> UpdateBookingOnlineRepo(BookingOnline bookingOnline)
        {
            return BookingOnlineDAO.Instance.UpdateBookingOnlineDao(bookingOnline);
        }
        public Task DeleteBookingOnlineRepo(string bookingOnlineId)
        {
            return BookingOnlineDAO.Instance.DeleteBookingOnlineDao(bookingOnlineId);
        }
        public Task<bool> CheckCustomerHasUncompletedBookingRepo(string customerId)
        {
            return BookingOnlineDAO.Instance.CheckCustomerHasUncompletedBookingDao(customerId);
        }
        public Task<BookingOnline> UpdateBookingOnlineStatusRepo(string bookingOnlineId, string status)
        {
            return BookingOnlineDAO.Instance.UpdateBookingOnlineStatusDao(bookingOnlineId, status);
        }

        public Task<BookingOnline> UpdateBookingOnlineMasterNoteRepo(string bookingOnlineId, string masterNote)
        {
            return BookingOnlineDAO.Instance.UpdateBookingOnlineMasterNoteDao(bookingOnlineId, masterNote);
        }
    }
}
