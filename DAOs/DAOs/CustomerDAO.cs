using BusinessObjects.Models;
using DAOs.DTOs;
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
        private readonly KoiFishPondContext _context;

        public CustomerDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<Customer> GetCustomerById(string customerId)
        {
            return await _context.Customers.FindAsync(customerId);
        }
        public async Task<ElementLifePalaceDto> GetElementLifePalaceById(string accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId))
            {
                return null;
            }

            var customer = await _context.Customers
                .Where(c => c.AccountId == accountId)
                .Select(c => new ElementLifePalaceDto
                {
                    Element = c.Element,
                    LifePalace = c.LifePalace
                })
                .FirstOrDefaultAsync();

            return customer;
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
