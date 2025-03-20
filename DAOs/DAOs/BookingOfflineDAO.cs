using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class BookingOfflineDAO
    {
        private static volatile BookingOfflineDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private BookingOfflineDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static BookingOfflineDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new BookingOfflineDAO();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<BookingOffline> GetBookingOfflineByIdDao(string bookingOfflineId)
        {
            return await _context.BookingOfflines
                .Include(x => x.Customer).ThenInclude(x => x.Account)
                .Include(x => x.Master).ThenInclude(x => x.Account)
                .Include(x => x.ConsultationPackage)
                .FirstOrDefaultAsync(x => x.BookingOfflineId == bookingOfflineId);
        }

        public async Task<List<BookingOffline>> GetBookingOfflinesDao()
        {
            return await _context.BookingOfflines
                .Include(x => x.Customer).ThenInclude(x => x.Account)
                .Include(x => x.Master).ThenInclude(x => x.Account)
                .ToListAsync();
        }

        public async Task<BookingOffline> GetConsultingOfflineByMasterScheduleIdDao(string masterScheduleId)
        {
            return await _context.BookingOfflines
                .Include(x => x.Customer).ThenInclude(x => x.Account)
                .Include(x => x.Master).ThenInclude(x => x.Account)
                .FirstOrDefaultAsync(x => x.MasterScheduleId == masterScheduleId);
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
