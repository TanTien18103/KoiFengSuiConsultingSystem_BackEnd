using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Exceptions;
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
                .Include(x => x.Contract)
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
            return _context.BookingOfflines.Include(x => x.Contract).Where(b => b.CustomerId == userId).ToList();
        }

        public async Task<List<BookingOffline>> GetBookingOfflinesByAccountIdDao(string accountId)
        {
            return await _context.BookingOfflines
                .Include(x => x.Contract)
                .Include(x => x.Customer).ThenInclude(x => x.Account)
                .Where(x => x.Customer.Account.AccountId == accountId)
                .ToListAsync();
        }

        public async Task<(BookingOffline booking, string message)> ProcessBookingTransactionDao(BookingOffline booking, string packageId, decimal selectedPrice)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var package = await _context.ConsultationPackages
                        .FirstOrDefaultAsync(x => x.ConsultationPackageId == packageId);

                    if (package == null)
                    {
                        throw new AppException(ResponseMessageConstrantsPackage.PACKAGE_NOT_FOUND);
                    }

                    if (selectedPrice != package.MinPrice && selectedPrice != package.MaxPrice)
                    {
                        throw new AppException(ResponseMessageConstrantsBooking.PRICE_SELECTED_INVALID);
                    }

                    _context.BookingOfflines.Add(booking);
                    await _context.SaveChangesAsync();

                    booking.ConsultationPackageId = packageId;
                    _context.BookingOfflines.Update(booking);
                    await _context.SaveChangesAsync();

                    booking.SelectedPrice = selectedPrice;
                    _context.BookingOfflines.Update(booking);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    await _context.Entry(booking)
                        .Reference(b => b.Customer)
                        .LoadAsync();
                    await _context.Entry(booking.Customer)
                        .Reference(c => c.Account)
                        .LoadAsync();
                    await _context.Entry(booking)
                        .Reference(b => b.ConsultationPackage)
                        .LoadAsync();

                    return (booking, "Transaction completed successfully");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return (null, ex.Message);
                }
            }
        }

        public async Task<BookingOffline> CreateBookingOfflineDao(BookingOffline bookingOffline)
        {
            _context.BookingOfflines.Add(bookingOffline);
            await _context.SaveChangesAsync();
            return bookingOffline;
        }

        public async Task<BookingOffline> UpdateBookingOfflineDao(BookingOffline bookingOffline)
        {
            var entity = new BookingOffline
            {
                BookingOfflineId = bookingOffline.BookingOfflineId
            };

            var local = _context.BookingOfflines.Local
                .FirstOrDefault(e => e.BookingOfflineId == entity.BookingOfflineId);

            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Attach(entity);

            var scalarProps = _context.Entry(entity).Metadata.GetProperties()
                .Select(p => p.Name)
                .Where(name => name != nameof(BookingOffline.BookingOfflineId));

            foreach (var propName in scalarProps)
            {
                var value = typeof(BookingOffline).GetProperty(propName)?.GetValue(bookingOffline);
                if (value != null)
                {
                    typeof(BookingOffline).GetProperty(propName)?.SetValue(entity, value);
                    _context.Entry(entity).Property(propName).IsModified = true;
                }
            }

            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteBookingOfflineDao(string bookingOfflineId)
        {
            var bookingOffline = await GetBookingOfflineByIdDao(bookingOfflineId);
            _context.BookingOfflines.Remove(bookingOffline);
            await _context.SaveChangesAsync();
        }
        public async Task<List<BookingOffline>> GetBookingOfflinesByMasterIdDao(string masterId)
        {
            try
            {
                return await _context.BookingOfflines
                    .Include(x => x.Customer).ThenInclude(x => x.Account)
                    .Include(x => x.Master).ThenInclude(x => x.Account)
                    .Include(x => x.ConsultationPackage)
                    .Include(x => x.Contract)
                    .Where(b => b.MasterId == masterId)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch
            {
                return new List<BookingOffline>();
            }
        }

        private void RefreshContext()
        {
            _context.ChangeTracker.Clear();
        }
        public async Task<List<BookingOffline>?> GetBookingsOfflineByCustomerIdDao(string customerId)
        {
            RefreshContext();
            return await _context.BookingOfflines
               .AsNoTracking()
               .Include(x => x.Customer).ThenInclude(x => x.Account)
               .Include(x => x.Master)
               .Where(b => b.CustomerId == customerId)
               .ToListAsync();
        }

        public async Task<List<BookingOffline>?> GetBookingsOfflineByStaffIdDao(string staffId)
        {
            return await _context.BookingOfflines
                   .Include(x => x.Customer).ThenInclude(x => x.Account)
                   .Include(x => x.Master)
           .Where(b => b.AssignStaffId == staffId)
           .ToListAsync();
        }

        public async Task<BookingOffline> GetPendingBookingByCustomerIdDao(string customerId)
        {
            return await _context.BookingOfflines
                .Include(x => x.Customer).ThenInclude(x => x.Account)
                .Include(x => x.Master).ThenInclude(x => x.Account)
                .Include(x => x.ConsultationPackage)
                .Include(x => x.Contract)
                .FirstOrDefaultAsync(b => b.CustomerId == customerId && b.Status == BookingOfflineEnums.Pending.ToString());
        }
    }
}
