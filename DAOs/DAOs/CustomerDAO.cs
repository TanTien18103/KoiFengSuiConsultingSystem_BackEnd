using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class CustomerDAO
    {
        private static volatile CustomerDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private CustomerDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static CustomerDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new CustomerDAO();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<Customer?> GetCustomerByIdDao(string customerId)
        {
            return await _context.Customers.Include(x => x.Account).FirstOrDefaultAsync(x => x.CustomerId == customerId);
        }

        public async Task<string> GetCustomerIdByAccountIdDao(string accountId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.AccountId == accountId);
            return customer.CustomerId;
        }

        public async Task<Customer?> GetCustomerByAccountIdDao(string accountId)
        {
            return await _context.Customers
                .Include(c => c.Account) 
                .FirstOrDefaultAsync(x => x.Account.AccountId == accountId);
        }

        public async Task<Customer> GetElementLifePalaceByIdDao(string accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId))
            {
                return null;
            }
        
            return await _context.Customers.Include(x => x.Account)
                .Where(c => c.AccountId == accountId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Customer>> GetCustomersDao()
        {
            return _context.Customers.ToList();
        }

        public async Task<Customer> CreateCustomerDao(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer> UpdateCustomerDao(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task DeleteCustomerDao(string customerId)
        {
            var customer = await GetCustomerByIdDao(customerId);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Customer>> GetAllCustomersDao()
        {
            return await _context.Customers
                .Include(c => c.Account)
                .Where(c => c.Account.IsActive)
                .ToListAsync();
        }
    }
}
