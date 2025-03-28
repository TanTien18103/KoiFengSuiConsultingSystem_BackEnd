using BusinessObjects.Enums;
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

        public async Task<bool> CheckCustomerHasUncompletedBookingDao(string customerId)
        {
            var customerBookings = await _context.BookingOnlines
                .Where(b => b.CustomerId == customerId)
                .ToListAsync();

            if (customerBookings.Count == 0)
                return false;

            foreach (var booking in customerBookings)
            {
                var hasOrder = await _context.Orders
                    .AnyAsync(o => o.ServiceId == booking.BookingOnlineId);

                if (!hasOrder)
                    return true; 
            }
            return false; 
        }

        public async Task<BookingOnline> UpdateBookingOnlineStatusDao(string bookingOnlineId, string status)
        {
            var bookingOnline = await GetBookingOnlineByIdDao(bookingOnlineId);
            if (bookingOnline != null)
            {
                bookingOnline.Status = status;
                _context.BookingOnlines.Update(bookingOnline);
                await _context.SaveChangesAsync();
            }
            return bookingOnline;
        }

        public async Task<BookingOnline> UpdateBookingOnlineMasterNoteDao(string bookingOnlineId, string masterNote)
        {
            try
            {
                var bookingOnline = await _context.BookingOnlines
                    .AsNoTracking()
                    .FirstOrDefaultAsync(b => b.BookingOnlineId == bookingOnlineId);

                if (bookingOnline == null)
                    return null;

                
                var entry = _context.BookingOnlines.FirstOrDefault(b => b.BookingOnlineId == bookingOnlineId);
                if (entry != null)
                {
                    entry.MasterNote = masterNote;
                    await _context.SaveChangesAsync();

                    
                    return await _context.BookingOnlines
                        .Include(x => x.Customer).ThenInclude(x => x.Account)
                        .Include(x => x.Master).ThenInclude(x => x.Account)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(b => b.BookingOnlineId == bookingOnlineId);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
        public async Task<List<BookingOnline>> GetBookingOnlinesByMasterIdDao(string masterId)
        {
            try
            {
                return await _context.BookingOnlines
                    .Include(x => x.Customer).ThenInclude(x => x.Account)
                    .Include(x => x.Master).ThenInclude(x => x.Account)
                    .Where(b => b.MasterId == masterId)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch
            {
                return new List<BookingOnline>();
            }
        }

        public async Task<List<BookingOnline>> GetConflictingBookingsDao(string masterId, DateOnly bookingDate, TimeOnly startTime)
        {
            return await _context.BookingOnlines
                .Where(b => b.MasterId == masterId &&
                           b.BookingDate == bookingDate &&
                           b.StartTime == startTime &&
                           b.Status != BookingOnlineEnums.Cancelled.ToString() &&
                           b.Status != BookingOnlineEnums.Completed.ToString())
                .ToListAsync();
        }

        public async Task<List<BookingOnline>> GetUnpaidBookingsOlderThanDao(DateTime cutoffDate)
        {
            return await _context.BookingOnlines
                .Where(b => b.Status == BookingOnlineEnums.Pending.ToString() &&
                           b.CreateDate < cutoffDate)
                .ToListAsync();
        }

        public async Task<BookingOnline> GetBookingOnlineByOrderIdDao(string orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order != null && order.ServiceType == PaymentTypeEnums.BookingOnline.ToString())
            {
                return await GetBookingOnlineByIdDao(order.ServiceId);
            }
            return null;
        }
    }
}
