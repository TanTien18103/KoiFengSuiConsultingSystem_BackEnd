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
        public Task<Customer> GetCustomerById(string customerId)
        {
            return CustomerDAO.Instance.GetCustomerByIdDao(customerId);
        }

        public Task<Customer> CreateCustomer(Customer customer)
        {
            return CustomerDAO.Instance.CreateCustomerDao(customer);
        }

        public Task<Customer> UpdateCustomer(Customer customer)
        {
            return CustomerDAO.Instance.UpdateCustomerDao(customer);
        }

        public Task DeleteCustomer(string customerId)
        {
            return CustomerDAO.Instance.DeleteCustomerDao(customerId);
        }

        public Task<List<Customer>> GetCustomers()
        {
            return CustomerDAO.Instance.GetCustomersDao();
        }

        public Task<Customer> GetElementLifePalaceById(string accountId)
        {
            return CustomerDAO.Instance.GetElementLifePalaceByIdDao(accountId);
        }
    }
}
