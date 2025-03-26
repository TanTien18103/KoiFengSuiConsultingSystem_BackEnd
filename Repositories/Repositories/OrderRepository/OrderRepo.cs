using BusinessObjects.Enums;
using BusinessObjects.Models;
using DAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

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
        public Task<Order> GetOrderByServiceId(string serviceId)
        {
            return OrderDAO.Instance.GetOrdersByServiceIdDao(serviceId);
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
        public Task<List<Order>> GetPendingOrdersByCustomerId(string customerId)
        {
            return OrderDAO.Instance.GetPendingOrdersByCustomerIdDao(customerId);
        }
        public Task<List<Order>> GetAllOrders()
        {
            return OrderDAO.Instance.GetAllOrdersDao();
        }
        public Task<List<Order>> GetOrdersByStatusAndCustomer(string status, string accountId)
        {
            return OrderDAO.Instance.GetOrdersByStatusAndCustomerDao(status, accountId);
        }
        public Task<Order> GetOrderByServiceIdAndStatus(string serviceId, string serviceType, string status)
        {
            return OrderDAO.Instance.GetOrderByServiceIdAndStatusDao(serviceId, serviceType, status);
        }
        public Task<Order> GetOrderByServiceIdAndCustomerIdAndServiceType(string customerId, string serviceId, string serviceType)
        {
            return OrderDAO.Instance.GetOrderByServiceIdAndCustomerIdAndServiceType(customerId, serviceId, serviceType);
        }
        public Task<List<Order>> GetOrdersByCustomerAndType(string customerId, string serviceType, string status)
        {
            return OrderDAO.Instance.GetOrdersByCustomerAndTypeDao(customerId, serviceType, status);
        }
        public Task<Order> GetOrderWithDetails(string orderId)
        {
            return OrderDAO.Instance.GetOrderWithDetailsDao(orderId);
        }
    }
}
