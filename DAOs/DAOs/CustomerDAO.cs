using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class CustomerDAO
    {
        private readonly KoiFishPondContext _context;

        public CustomerDAO()
        {
            _context = new KoiFishPondContext();
        }

        public async Task<Customer> GetCustomerById(string customerId)
        {
            return await _context.Customers.FindAsync(customerId);
        }

        public async Task<List<Customer>> GetCustomers()
        {
            return _context.Customers.ToList();
        }

        public async Task<Customer> CreateCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer> UpdateCustomer(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task DeleteCustomer(string customerId)
        {
            var customer = await GetCustomerById(customerId);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }
}
