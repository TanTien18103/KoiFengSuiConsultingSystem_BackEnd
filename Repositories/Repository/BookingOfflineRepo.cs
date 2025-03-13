using BusinessObjects.Models;
using DAOs.DAOs;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
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

        public Task<List<BookingOffline>> GetBookingOfflinesByUserId(string userId)
        {
            return BookingOfflineDAO.Instance.GetBookingOfflinesByUserIdDao(userId);
        }
        public Task<List<BookingOffline>> GetBookingOfflines()
        {
            return BookingOfflineDAO.Instance.GetBookingOfflinesDao();
        }
    }
}
