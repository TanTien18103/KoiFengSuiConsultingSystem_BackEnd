using BusinessObjects.Models;
using DAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DAOs.DAOs.BookingOnlineDAO;

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
        public Task<BookingCheckResult> CheckCustomerHasUncompletedBookingRepo(string customerId)
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
        public Task<List<BookingOnline>> GetBookingOnlinesByMasterIdRepo(string masterId)
        {
            return BookingOnlineDAO.Instance.GetBookingOnlinesByMasterIdDao(masterId);
        }
        public Task<List<BookingOnline>> GetConflictingBookingsRepo(string masterId, DateOnly bookingDate, TimeOnly startTime)
        {
            return BookingOnlineDAO.Instance.GetConflictingBookingsDao(masterId, bookingDate, startTime);
        }
        public Task<List<BookingOnline>> GetUnpaidBookingsOlderThanRepo(DateTime cutoffDate)
        {
            return BookingOnlineDAO.Instance.GetUnpaidBookingsOlderThanDao(cutoffDate);
        }

        public Task<BookingOnline> GetBookingOnlineByOrderIdRepo(string orderId)
        {
            return BookingOnlineDAO.Instance.GetBookingOnlineByOrderIdDao(orderId);
        }

        public Task<BookingOnline> GetBookingOnlineByMasterScheduleIdRepo(string masterScheduleId)
        {
            return BookingOnlineDAO.Instance.GetBookingOnlineByMasterScheduleIdDao(masterScheduleId);
        }

        public Task<List<BookingOnline>> GetBookingsByMasterAndTimeRepo(string masterId, TimeOnly startTime, TimeOnly endTime, DateOnly bookingDate)
        {
            return BookingOnlineDAO.Instance.GetBookingsByMasterAndTimeDao(masterId, startTime, endTime, bookingDate);
        }
        public Task<BookingOnline> UpdateBookingOnlineWithTrackingRepo(BookingOnline bookingOnline)
        {
            return BookingOnlineDAO.Instance.UpdateBookingOnlineWithTrackingDao(bookingOnline);
        }

        public Task<List<BookingOnline>?> GetBookingsOnlineByCustomerId(string customerId)
        {
            return BookingOnlineDAO.Instance.GetBookingsOnlineByCustomerIdDao(customerId);
        }

        public Task<List<BookingOnline>?> GetBookingsOnlineByStaffId(string staffId)
        {
            return BookingOnlineDAO.Instance.GetBookingsOnlineByStaffIdDao(staffId);
        }
    }
}
