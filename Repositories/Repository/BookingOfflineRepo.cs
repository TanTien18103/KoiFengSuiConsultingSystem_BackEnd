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
        private readonly BookingOfflineDAO _bookingOfflineDAO;

        public BookingOfflineRepo(BookingOfflineDAO bookingOfflineDAO)
        {
            _bookingOfflineDAO = bookingOfflineDAO;
        }

        public async Task<BookingOffline> GetBookingOfflineById(string bookingOfflineId)
        {
            return await _bookingOfflineDAO.GetBookingOfflineById(bookingOfflineId);
        }

       

        public async Task<BookingOffline> CreateBookingOffline(BookingOffline bookingOffline)
        {
            return await _bookingOfflineDAO.CreateBookingOffline(bookingOffline);
        }

        public async Task<BookingOffline> UpdateBookingOffline(BookingOffline bookingOffline)
        {
            return await _bookingOfflineDAO.UpdateBookingOffline(bookingOffline);
        }

        public async Task DeleteBookingOffline(string bookingOfflineId)
        {
            await _bookingOfflineDAO.DeleteBookingOffline(bookingOfflineId);
        }

        public async Task<List<BookingOffline>> GetBookingOfflinesByUserId(string userId)
        {
            return await _bookingOfflineDAO.GetBookingOfflinesByUserId(userId);
        }
    }
}
