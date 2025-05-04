using BusinessObjects.Enums;
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
        Task<ResultModel> GetWaitingForRefundOrders();
        Task<ResultModel> CancelOrder(string serviceId, PaymentTypeEnums serviceType);
        Task<ResultModel> CheckAndUpdateExpiredOrders();
        Task<ResultModel> GetPendingConfirmOrders();
        Task<ResultModel> GetDetailsOrder(string id);

        //refund
        Task<ResultModel> GetManagerRefunded();
        Task<ResultModel> ManagerConfirmRefunded(string id);
        Task<ResultModel> CustomerConfirmReceived(string id);
    }
}
