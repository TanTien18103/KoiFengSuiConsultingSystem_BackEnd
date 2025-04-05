using Services.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.OrderService
{
    public interface IOrderService
    {
        Task<ResultModel> UpdateOrderToPaid(string id);
        Task<ResultModel> UpdateOrderToPendingConfirm(string id);
        Task<ResultModel> GetPendingOrders();
        Task<ResultModel> CancelOrder(string orderId);
        Task<ResultModel> CheckAndUpdateExpiredOrders();
        Task<ResultModel> GetPendingConfirmOrders();
        Task<ResultModel> GetDetailsOrder(string id);
    }
}
