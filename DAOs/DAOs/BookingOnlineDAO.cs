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
        private static volatile BookingOnlineDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private BookingOnlineDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static BookingOnlineDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new BookingOnlineDAO();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<BookingOnline> GetBookingOnlineByIdDao(string bookingOnlineId)
        {
            var booking = await _context.BookingOnlines
                .Include(x => x.Customer).ThenInclude(x => x.Account)
                .Include(x => x.Master).ThenInclude(x => x.Account)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.BookingOnlineId.Equals(bookingOnlineId));
            return booking;
        }

        public async Task<BookingOnline> GetConsultingOnlineByMasterScheduleIdDao(string masterScheduleId)
        {
            return await _context.BookingOnlines
                .Include(x => x.Customer).ThenInclude(x => x.Account)
                .Include(x => x.Master).ThenInclude(x => x.Account)
                .FirstOrDefaultAsync(x => x.MasterScheduleId == masterScheduleId);
        }

        public async Task<List<BookingOnline>> GetBookingOnlinesDao()
        {

            return await _context.BookingOnlines
                .Include(x => x.Customer).ThenInclude(x => x.Account)
                .Include(x => x.Master).ThenInclude(x => x.Account)
                .AsNoTracking()
                .ToListAsync();
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
