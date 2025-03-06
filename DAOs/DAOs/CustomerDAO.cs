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
        public static CustomerDAO instance = null;
        private readonly KoiFishPondContext _context;

        public CustomerDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static CustomerDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CustomerDAO();
                }
                return instance;
            }
        }

        public async Task<Customer?> GetCustomerByIdDao(string customerId)
        {
            return await _context.Customers.FirstOrDefaultAsync(x => x.CustomerId == customerId);
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
        
            return await _context.Customers
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
    }
}
