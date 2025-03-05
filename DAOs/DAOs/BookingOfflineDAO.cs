using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class BookingOfflineDAO
    {
        public static BookingOfflineDAO instance = null;
        private readonly KoiFishPondContext _context;

        public BookingOfflineDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static BookingOfflineDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BookingOfflineDAO();
                }
                return instance;
            }
        }

        public async Task<BookingOffline> GetBookingOfflineByIdDao(string bookingOfflineId)
        {
            return await _context.BookingOfflines.FindAsync(bookingOfflineId);
        }

        public async Task<List<BookingOffline>> GetBookingOfflinesByUserIdDao(string userId)
        {
            return _context.BookingOfflines.Where(b => b.CustomerId == userId).ToList();
        }

        public async Task<BookingOffline> CreateBookingOfflineDao(BookingOffline bookingOffline)
        {
            _context.BookingOfflines.Add(bookingOffline);
            await _context.SaveChangesAsync();
            return bookingOffline;
        }

        public async Task<BookingOffline> UpdateBookingOfflineDao(BookingOffline bookingOffline)
        {
            _context.BookingOfflines.Update(bookingOffline);
            await _context.SaveChangesAsync();
            return bookingOffline;
        }

        public async Task DeleteBookingOfflineDao(string bookingOfflineId)
        {
            var bookingOffline = await GetBookingOfflineByIdDao(bookingOfflineId);
            _context.BookingOfflines.Remove(bookingOffline);
            await _context.SaveChangesAsync();
        }


    }
}
