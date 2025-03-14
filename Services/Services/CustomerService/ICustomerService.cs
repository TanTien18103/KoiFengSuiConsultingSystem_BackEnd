using BusinessObjects.Models;
using Services.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.CustomerService
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomerById(string customerId);
        Task<ResultModel> GetCurrentCustomerElement();
        Task<List<Customer>> GetCustomers();
        Task<Customer> CreateCustomer(Customer customer);
        Task<Customer> UpdateCustomer(Customer customer);
        Task DeleteCustomer(string customerId);
        Task<ResultModel> CalculateCompatibility(CompatibilityRequest request);
    }
}
