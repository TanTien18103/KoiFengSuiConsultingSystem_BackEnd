using BusinessObjects.Enums;
using BusinessObjects.Models;
using DAOs.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class OrderDAO
    {
        private static volatile OrderDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private OrderDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static OrderDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new OrderDAO();
                        }
                    }
                }
                return _instance;
            }
        }
        public async Task<List<Order>> GetAllOrdersDao()
        {
            return await _context.Orders
                .Include(x => x.Customer).ThenInclude(x => x.Account)
                .ToListAsync();
        }

        public async Task<Order> CreateOrderDao(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<List<Order>> GetOrdersByStatusAndCustomerDao(string status, string accountId)
        {
            return await _context.Orders
                .Where(o => o.Status == status && o.Customer != null && o.Customer.Account != null && o.Customer.Account.AccountId == accountId)
                .ToListAsync();
        }

        public async Task<Order> GetOrderByIdDao(string id)
        {
            return await _context.Orders
                .Include(x => x.Customer).ThenInclude(x => x.Account)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task<Order> GetOrderByOrderCodeDao(string orderCode)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.OrderCode == orderCode);
        }

        public async Task<Order> GetOrderWithDetailsDao(string orderId)
        {
            return await _context.Orders
                .Include(o => o.Customer).ThenInclude(c => c.Account)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<Order> GetOrderByServiceIdAndStatusDao(string serviceId, string serviceType, string status)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o =>
                    o.ServiceId == serviceId &&
                    o.ServiceType == serviceType &&
                    o.Status == status);
        }

        public async Task<List<Order>> GetOrdersByCustomerIdDao(string customerId)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByServiceDao(string serviceId, PaymentTypeEnums serviceType)
        {
            return await _context.Orders
                .Where(o => o.ServiceId == serviceId && o.ServiceType == serviceType.ToString())
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<Order> GetOneOrdersByServiceDao(string serviceId, PaymentTypeEnums serviceType)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.ServiceId == serviceId && o.ServiceType == serviceType.ToString());
        }

        public async Task<Order> GetOrdersByServiceIdDao(string serviceId)
        {
            return await _context.Orders
                .OrderByDescending(o => o.CreatedDate)
                .FirstOrDefaultAsync(o => o.ServiceId == serviceId);
        }

        public async Task<bool> UpdateOrderDao(Order order)
        {
            _context.Orders.Update(order);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteOrderDao(string id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return false;

            _context.Orders.Remove(order);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Order>> GetPendingOrdersByCustomerIdDao(string customerId)
        {
            return await _context.Orders
                .AsNoTracking()
                .Where(o => o.CustomerId == customerId &&
                           (o.Status == PaymentStatusEnums.Pending.ToString() ||
                           o.Status == PaymentStatusEnums.PendingConfirm.ToString()))
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<Order> GetOrderByServiceIdAndCustomerIdAndServiceType(string customerId, string serviceId, string serviceType)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o =>
                    o.CustomerId == customerId &&
                    o.ServiceId == serviceId &&
                    o.ServiceType == serviceType);
        }

        public async Task<List<Order>> GetOrdersByCustomerAndTypeDao(string customerId, string serviceType, string status)
        {
            return await _context.Orders
                .Where(o =>
                    o.CustomerId == customerId &&
                    o.ServiceType == serviceType &&
                    o.Status == status)
                .OrderByDescending(o => o.PaymentDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalRevenueDao()
        {
            return await _context.Orders
                .Where(o => o.Status == PaymentStatusEnums.Paid.ToString())
                .SumAsync(o => o.Amount) ?? 0m;
        }

        public async Task<List<MonthlyServiceStatisticsDto>> GetMonthlyServiceStatisticsDao()
        {
            var rawData = await _context.Orders
                .Where(o =>
                    (o.Status == PaymentStatusEnums.Paid.ToString() ||
                     o.Status == PaymentStatusEnums.Paid2nd.ToString()) &&
                     o.CreatedDate.HasValue)
                .GroupBy(o => new { Month = o.CreatedDate.Value.Month, Year = o.CreatedDate.Value.Year })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Courses = g.Count(x => x.ServiceType == PaymentTypeEnums.Course.ToString()),
                    Workshops = g.Count(x => x.ServiceType == PaymentTypeEnums.RegisterAttend.ToString()),
                    BookingOnline = g.Count(x => x.ServiceType == PaymentTypeEnums.BookingOnline.ToString()),
                    BookingOffline = g.Count(x => x.ServiceType == PaymentTypeEnums.BookingOffline.ToString())
                })
                .ToListAsync();

            return rawData
                .Select(item => new MonthlyServiceStatisticsDto
                {
                    MonthYear = $"{item.Month:00}/{item.Year}",
                    Courses = item.Courses,
                    Workshops = item.Workshops,
                    BookingOnline = item.BookingOnline,
                    BookingOffline = item.BookingOffline
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();
        }

        public async Task<List<GetTodayTimeAdmittedDto>> GetTodayTimeAdmittedDao()
        {
            var today = DateTime.Today;

            var data = await _context.Orders
                .Where(o =>
                       o.CreatedDate.HasValue &&
                       o.CreatedDate.Value.Date == today && (
                       o.Status == PaymentStatusEnums.Paid.ToString() || 
                       o.Status == PaymentStatusEnums.Paid2nd.ToString()))
                .GroupBy(o => o.CreatedDate.Value.Hour)
                .Select(g => new
                {
                    Hour = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Hour)
                .ToListAsync();

            return data.Select(x => new GetTodayTimeAdmittedDto
            {
                Time = $"{x.Hour:00}:00",
                Count = x.Count
            }).ToList();
        }

        public async Task<int> GetTodayCourseCountDao()
        {
            var today = DateTime.Today;

            return await _context.Orders
                .CountAsync(o => o.CreatedDate.Value.Date == today && (
                o.Status == PaymentStatusEnums.Paid.ToString() ||
                o.Status == PaymentStatusEnums.Paid.ToString()) && 
                o.ServiceType == PaymentTypeEnums.Course.ToString());
        }

        public async Task<int> GetTodayWorkshopCheckInCountDao()
        {
            var today = DateTime.Today;

            return await _context.Orders
                .CountAsync(o => o.CreatedDate.Value.Date == today
                && o.Status == PaymentStatusEnums.Paid.ToString()
                && o.ServiceType == PaymentTypeEnums.RegisterAttend.ToString());
        }

        public async Task<int> GetTodayBookingOfflineCountDao()
        {
            var today = DateTime.Today;

            return await _context.Orders
                 .CountAsync(o => o.CreatedDate.Value.Date == today
                && o.Status == PaymentStatusEnums.Paid2nd.ToString()
                && o.ServiceType == PaymentTypeEnums.BookingOffline.ToString());

        }

        public async Task<int> GetTodayBookingOnlineCountDao()
        {
            var today = DateTime.Today;

            return await _context.Orders
                .CountAsync(o => o.CreatedDate.Value.Date == today
                && o.Status == PaymentStatusEnums.Paid.ToString()
                && o.ServiceType == PaymentTypeEnums.BookingOnline.ToString());
        }
    }
}
