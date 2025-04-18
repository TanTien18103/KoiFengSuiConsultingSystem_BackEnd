using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.CustomerRepository
{
    public interface ICustomerRepo
    {
        Task<Customer> GetCustomerById(string customerId);
        Task<Customer> GetCustomerByAccountId(string id);
        Task<Customer> GetElementLifePalaceById(string accountId);
        Task<List<Customer>> GetCustomers();
        Task<string> GetCustomerIdByAccountId(string accountId);
        Task<Customer> CreateCustomer(Customer customer);
        Task<Customer> UpdateCustomer(Customer customer);
        Task DeleteCustomer(string customerId);
        Task<List<Customer>> GetAllCustomers();
        Task<bool> DeleteCustomerByAccountId(string accountId);
    }
}
