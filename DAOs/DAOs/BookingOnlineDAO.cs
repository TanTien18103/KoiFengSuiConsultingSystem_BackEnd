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
            return await _context.BookingOnlines
                .Include(x => x.Customer).ThenInclude(x => x.Account)
                .Include(x => x.Master).ThenInclude(x => x.Account)
                .FirstOrDefaultAsync(b => b.BookingOnlineId == bookingOnlineId);
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

        public class BookingCheckResult
        {
            public bool HasPendingBooking { get; set; }
            public bool HasPendingConfirmBooking { get; set; }
        }

        public async Task<BookingCheckResult> CheckCustomerHasUncompletedBookingDao(string customerId)
        {
            var result = new BookingCheckResult();

            // Lấy tất cả booking Pending của customer
            var customerBookings = await _context.BookingOnlines
                .Where(b => b.CustomerId == customerId && b.Status == BookingOnlineEnums.Pending.ToString())
                .ToListAsync();

            if (!customerBookings.Any())
                return result;

            foreach (var booking in customerBookings)
            {
                var order = await _context.Orders
                    .FirstOrDefaultAsync(o =>
                        o.ServiceId == booking.BookingOnlineId &&
                        o.ServiceType == PaymentTypeEnums.BookingOnline.ToString());

                if (order == null || order.Status == PaymentStatusEnums.Pending.ToString())
                {
                    result.HasPendingBooking = true;
                }
                else if (order.Status == PaymentStatusEnums.PendingConfirm.ToString())
                {
                    result.HasPendingConfirmBooking = true;
                }
            }

            return result;
        }

        public async Task<BookingOnline> UpdateBookingOnlineStatusDao(string bookingOnlineId, string status)
        {
            var booking = await _context.BookingOnlines
                .FirstOrDefaultAsync(b => b.BookingOnlineId == bookingOnlineId);

            if (booking != null)
            {
                booking.Status = status;
                await _context.SaveChangesAsync();
            }
            return booking;
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
                           b.Status != BookingOnlineEnums.Canceled.ToString() &&
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

        public async Task<BookingOnline> GetBookingOnlineByMasterScheduleIdDao(string masterScheduleId)
        {
            return await _context.BookingOnlines
                .FirstOrDefaultAsync(b => 
                    b.MasterScheduleId == masterScheduleId && 
                    b.Status != BookingOnlineEnums.Canceled.ToString());
        }

        public async Task<List<BookingOnline>> GetBookingsByMasterAndTimeDao(string masterId, TimeOnly startTime, TimeOnly endTime, DateOnly bookingDate)
        {
            return await _context.BookingOnlines
                .Where(b => b.MasterId == masterId &&
                           b.BookingDate == bookingDate &&
                           ((startTime >= b.StartTime && startTime < b.EndTime) || 
                            (endTime > b.StartTime && endTime <= b.EndTime) ||     
                            (startTime <= b.StartTime && endTime >= b.EndTime) || 
                            (startTime <= b.EndTime && endTime >= b.StartTime)) && 
                           b.Status != BookingOnlineEnums.Canceled.ToString()) 
                .ToListAsync();
        }

        public async Task<BookingOnline> UpdateBookingOnlineWithTrackingDao(BookingOnline bookingOnline)
        {
            var currentBooking = await _context.BookingOnlines.AsNoTracking().FirstOrDefaultAsync(b => b.BookingOnlineId == bookingOnline.BookingOnlineId);

            if (currentBooking == null)
            {
                throw new Exception("Không tìm thấy booking để cập nhật");
            }

            _context.Entry(bookingOnline).State = EntityState.Detached;

            _context.BookingOnlines.Update(bookingOnline);
            await _context.SaveChangesAsync();
            return bookingOnline;
        }

        public async Task<List<BookingOnline>?> GetBookingsOnlineByCustomerIdDao(string customerId)
        {
            return await _context.BookingOnlines
                   .Where(b => b.CustomerId == customerId)
                   .ToListAsync();
        }

        public async Task<List<BookingOnline>?> GetBookingsOnlineByStaffIdDao(string staffId)
        {
            return await _context.BookingOnlines
                   .Include(x => x.Customer).ThenInclude(x => x.Account)
                   .Include(x => x.Master)
                   .Where(b => b.AssignStaffId == staffId)
                   .ToListAsync();
             return await _context.BookingOnlines
                    .Include(x => x.Customer).ThenInclude(x => x.Account)
                    .Include(x => x.Master)
                    .Where(b => b.CustomerId == customerId)
                    .ToListAsync();
        }
    }
}
