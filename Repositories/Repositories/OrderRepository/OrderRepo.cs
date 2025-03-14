using BusinessObjects.Enums;
using BusinessObjects.Models;
using DAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.OrderRepository
{
    public class OrderRepo : IOrderRepo
    {
        public Task<Order> GetOrderById(string id)
        {
            return OrderDAO.Instance.GetOrderByIdDao(id);
        }
        public Task<Order> GetOrderByOrderCode(string orderCode)
        {
            return OrderDAO.Instance.GetOrderByOrderCodeDao(orderCode);
        }
        public Task<List<Order>> GetOrderByCustomerId(string customerId)
        {
            return OrderDAO.Instance.GetOrdersByCustomerIdDao(customerId);
        }
        public Task<List<Order>> GetOrderByService(string serviceId, PaymentTypeEnums serviceType)
        {
            return OrderDAO.Instance.GetOrdersByServiceDao(serviceId, serviceType);
        }
        public Task<Order> CreateOrder(Order order)
        {
            return OrderDAO.Instance.CreateOrderDao(order);
        }
        public Task<bool> UpdateOrder(Order order)
        {
            return OrderDAO.Instance.UpdateOrderDao(order);
        }
        public Task<bool> DeleteOrder(string id)
        {
            return OrderDAO.Instance.DeleteOrderDao(id);
        }
    }
}
