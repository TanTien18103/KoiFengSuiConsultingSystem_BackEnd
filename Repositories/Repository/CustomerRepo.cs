using BusinessObjects.Models;
using DAOs.DAOs;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class CustomerRepo : ICustomerRepo
    {
        private readonly CustomerDAO _customerDAO;

        public CustomerRepo()
        {
            _customerDAO = new CustomerDAO();
        }

        public async Task<Customer> GetCustomerById(string customerId)
        {
            return await _customerDAO.GetCustomerById(customerId);
        }

        public async Task<Customer> CreateCustomer(Customer customer)
        {
            return await _customerDAO.CreateCustomer(customer);
        }

        public async Task<Customer> UpdateCustomer(Customer customer)
        {
            return await _customerDAO.UpdateCustomer(customer);
        }

        public async Task DeleteCustomer(string customerId)
        {
            await _customerDAO.DeleteCustomer(customerId);
        }

        public async Task<List<Customer>> GetCustomers()
        {
            return await _customerDAO.GetCustomers();
        }


    }
}
