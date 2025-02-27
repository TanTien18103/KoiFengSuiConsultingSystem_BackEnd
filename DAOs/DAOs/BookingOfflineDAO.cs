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
        private readonly KoiFishPondContext _context;


        public BookingOfflineDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<BookingOffline> GetBookingOfflineById(string bookingOfflineId)
        {
            return await _context.BookingOfflines.FindAsync(bookingOfflineId);
        }

        public async Task<List<BookingOffline>> GetBookingOfflinesByUserId(string userId)
        {
            return _context.BookingOfflines.Where(b => b.CustomerId == userId).ToList();
        }

        public async Task<BookingOffline> CreateBookingOffline(BookingOffline bookingOffline)
        {
            _context.BookingOfflines.Add(bookingOffline);
            await _context.SaveChangesAsync();
            return bookingOffline;
        }

        public async Task<BookingOffline> UpdateBookingOffline(BookingOffline bookingOffline)
        {
            _context.BookingOfflines.Update(bookingOffline);
            await _context.SaveChangesAsync();
            return bookingOffline;
        }

        public async Task DeleteBookingOffline(string bookingOfflineId)
        {
            var bookingOffline = await GetBookingOfflineById(bookingOfflineId);
            _context.BookingOfflines.Remove(bookingOffline);
            await _context.SaveChangesAsync();
        }


    }
}
