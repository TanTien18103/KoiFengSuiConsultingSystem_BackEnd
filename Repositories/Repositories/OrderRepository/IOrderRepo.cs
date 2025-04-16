using BusinessObjects.Enums;
using BusinessObjects.Models;
using DAOs.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.OrderRepository
{
    public interface IOrderRepo
    {
        Task<Order> GetOrderById(string id);
        Task<Order> GetOrderByOrderCode(string orderCode);
        Task<List<Order>> GetOrderByCustomerId(string customerId);
        Task<List<Order>> GetOrderByService(string serviceId, PaymentTypeEnums serviceType);
        Task<Order> GetOneOrderByService(string serviceId, PaymentTypeEnums serviceType);
        Task<Order> GetOrderByServiceId(string serviceId);
        Task<Order> CreateOrder(Order order);
        Task<bool> UpdateOrder(Order order);
        Task<bool> DeleteOrder(string id);
        Task<List<Order>> GetPendingOrdersByCustomerId(string customerId);
        Task<List<Order>> GetAllOrders();
        Task<List<Order>> GetOrdersByStatusAndCustomer(string status, string accountId);
        Task<Order> GetOrderByServiceIdAndStatus(string serviceId, string serviceType, string status);
        Task<Order> GetOrderByServiceIdAndCustomerIdAndServiceType(string customerId, string serviceId, string serviceType);
        Task<List<Order>> GetOrdersByCustomerAndType(string customerId, string serviceType, string status);
        Task<Order> GetOrderWithDetails(string orderId);
        Task<decimal> GetTotalRevenue();
        Task<List<MonthlyServiceStatisticsDto>> GetMonthlyServiceStatistics();
        Task<List<GetTodayTimeAdmittedDto>> GetTodayTimeAdmitted();
        Task<int> GetTodayCourseCount();
        Task<int> GetTodayWorkshopCheckInCount();
        Task<int> GetTodayBookingOnlineCount();
        Task<int> GetTodayBookingOfflineCount();
    }
}
