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
    public class BookingOnlineRepo : IBookingOnlineRepo
    {
        private readonly BookingOnlineDAO _bookingOnlineDAO;

        public BookingOnlineRepo()
        {
            _bookingOnlineDAO = new BookingOnlineDAO();
        }

        public async Task<BookingOnline> CreateBookingOnlineRepo(BookingOnline bookingOnline)
        {
            return await _bookingOnlineDAO.CreateBookingOnlineDao(bookingOnline);
        }

        public async Task DeleteBookingOnlineRepo(string bookingOnlineId)
        {
            await _bookingOnlineDAO.DeleteBookingOnlineDao(bookingOnlineId);
        }

        public async Task<BookingOnline> GetBookingOnlineByIdRepo(string bookingOnlineId)
        {
            return await _bookingOnlineDAO.GetBookingOnlineByIdDao(bookingOnlineId);
        }

        public async Task<List<BookingOnline>> GetBookingOnlinesRepo()
        {
            return await _bookingOnlineDAO.GetBookingOnlinesDao();
        }

        public async Task<BookingOnline> UpdateBookingOnlineRepo(BookingOnline bookingOnline)
        {
            return await _bookingOnlineDAO.UpdateBookingOnlineDao(bookingOnline);
        }
    }
}
