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
            return await _context.Orders.Include(x => x.Customer).ToListAsync();
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
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task<Order> GetOrderByOrderCodeDao(string orderCode)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.OrderCode == orderCode);
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
                .Where(o => o.CustomerId == customerId && 
                           o.Status == PaymentStatusEnums.Pending.ToString())
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }
    }
}
