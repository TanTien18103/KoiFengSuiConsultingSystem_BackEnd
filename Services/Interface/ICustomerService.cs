using BusinessObjects.Models;
using DAOs.DTOs;
using KoiFengSuiConsultingSystem.Request;
using KoiFengSuiConsultingSystem.Respone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomerById(string customerId);
        Task<ElementLifePalaceDto> GetElementLifePalaceById();
        Task<List<Customer>> GetCustomers();
        Task<Customer> CreateCustomer(Customer customer);
        Task<Customer> UpdateCustomer(Customer customer);
        Task DeleteCustomer(string customerId);
        Task<FengShuiResult> CalculateCompatibility(CompatibilityRequest request);
    }
}
