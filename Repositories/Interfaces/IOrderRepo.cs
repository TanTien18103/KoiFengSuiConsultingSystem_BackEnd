using BusinessObjects.Enums;
using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IOrderRepo
    {
        Task<Order> CreateOrder(Order order);
        Task<Order> GetOrderById(string id);
        Task<Order> GetOrderByOrderCode(string orderCode);
        Task<List<Order>> GetOrderByCustomerId(string customerId);
        Task<List<Order>> GetOrderByService(string serviceId, PaymentTypeEnums serviceType);
        Task<bool> DeleteOrder(string id);
        Task<bool> UpdateOrder(Order order);
    }
}
