using BusinessObjects.Models;
using DAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.BookingOfflineRepository
{
    public class BookingOfflineRepo : IBookingOfflineRepo
    {
        public Task<BookingOffline> GetBookingOfflineById(string bookingOfflineId)
        {
            return BookingOfflineDAO.Instance.GetBookingOfflineByIdDao(bookingOfflineId);
        }
        public Task<BookingOffline> GetConsultingOfflineByMasterScheduleIdRepo(string masterScheduleId)
        {
            return BookingOfflineDAO.Instance.GetConsultingOfflineByMasterScheduleIdDao(masterScheduleId);
        }
        public Task<List<BookingOffline>> GetBookingOfflinesByUserId(string userId)
        {
            return BookingOfflineDAO.Instance.GetBookingOfflinesByUserIdDao(userId);
        }
        public Task<List<BookingOffline>> GetBookingOfflinesByAccountId(string accountId)
        {
            return BookingOfflineDAO.Instance.GetBookingOfflinesByAccountIdDao(accountId);
        }
        public Task<List<BookingOffline>> GetBookingOfflines()
        {
            return BookingOfflineDAO.Instance.GetBookingOfflinesDao();
        }
        public Task<BookingOffline> CreateBookingOffline(BookingOffline bookingOffline)
        {
            return BookingOfflineDAO.Instance.CreateBookingOfflineDao(bookingOffline);
        }
        public Task<BookingOffline> UpdateBookingOffline(BookingOffline bookingOffline)
        {
            return BookingOfflineDAO.Instance.UpdateBookingOfflineDao(bookingOffline);
        }
        public Task DeleteBookingOffline(string bookingOfflineId)
        {
            return BookingOfflineDAO.Instance.DeleteBookingOfflineDao(bookingOfflineId);
        }
        public Task<(BookingOffline booking, string message)> ProcessBookingTransaction(BookingOffline booking, string packageId, decimal selectedPrice)
        {
            return BookingOfflineDAO.Instance.ProcessBookingTransactionDao(booking, packageId, selectedPrice);
        }
    }
}
