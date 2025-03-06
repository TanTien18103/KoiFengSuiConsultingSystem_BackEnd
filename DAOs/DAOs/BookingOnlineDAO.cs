using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class BookingOnlineDAO
    {
        public static BookingOnlineDAO instance = null;

        private readonly KoiFishPondContext _context;

        public BookingOnlineDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static BookingOnlineDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BookingOnlineDAO();
                }
                return instance;
            }
        }

        public async Task<BookingOnline> GetBookingOnlineByIdDao(string bookingOnlineId)
        {
            return await _context.BookingOnlines
                   .Include(b => b.Customer)
                   .ThenInclude(c => c.Account)
                   .FirstOrDefaultAsync(b => b.BookingOnlineId == bookingOnlineId);
        }

        public async Task<List<BookingOnline>> GetBookingOnlinesDao()
        {
            var bookings = await _context.BookingOnlines
            .Include(b => b.Customer)
            .ThenInclude(c => c.Account)
            .ToListAsync();
            return bookings;
        }

        public async Task<BookingOnline> CreateBookingOnlineDao(BookingOnline bookingOnline)
        {
            _context.BookingOnlines.Add(bookingOnline);
            await _context.SaveChangesAsync();
            return bookingOnline;
        }

        public async Task<BookingOnline> UpdateBookingOnlineDao(BookingOnline bookingOnline)
        {
            _context.BookingOnlines.Update(bookingOnline);
            await _context.SaveChangesAsync();
            return bookingOnline;
        }

        public async Task DeleteBookingOnlineDao(string bookingOnlineId)
        {
            var bookingOnline = await GetBookingOnlineByIdDao(bookingOnlineId);
            _context.BookingOnlines.Remove(bookingOnline);
            await _context.SaveChangesAsync();
        }



    }
}
