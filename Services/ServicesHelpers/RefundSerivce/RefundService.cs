using BusinessObjects.Constants;
using BusinessObjects.Enums;
using BusinessObjects.Exceptions;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Repositories.Repositories.AccountRepository;
using Repositories.Repositories.MasterRepository;
using Repositories.Repositories.OrderRepository;
using Services.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static BusinessObjects.Constants.ResponseMessageConstrantsKoiPond;

namespace Services.ServicesHelpers.RefundSerivce
{
    public class RefundService : IRefundService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMasterRepo _masterRepo;
        private readonly IAccountRepo _accountRepo;

        public RefundService(IOrderRepo orderRepo, IHttpContextAccessor httpContextAccessor, IMasterRepo masterRepo, IAccountRepo accountRepo) 
        { 
            _orderRepo = orderRepo;
            _httpContextAccessor = httpContextAccessor;
            _masterRepo = masterRepo;
            _accountRepo = accountRepo;
        }

        private string GetAuthenticatedAccountId()
        {
            var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            if (identity == null || !identity.IsAuthenticated) return null;
            return identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public async Task<string> ProcessRefundAsync(RefundRequest request)
        {
            var order = await _orderRepo.GetOrderWithDetails(request.OrderId);
            if (order == null)
                throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstrantsOrder.NOT_FOUND, StatusCodes.Status404NotFound);

            if (order.CustomerId != request.CustomerId)
            {
                throw new AppException(ResponseCodeConstants.BAD_REQUEST, ResponseMessageConstrantsOrder.WRONG_ORDER, StatusCodes.Status400BadRequest);
            }

            if (order.Customer?.Account == null)
                throw new AppException(ResponseCodeConstants.BAD_REQUEST, ResponseMessageConstantsUser.CUSTOMER_BANK_INFO_NOT_FOUND, StatusCodes.Status400BadRequest);

            if (order.ServiceType == PaymentTypeEnums.BookingOffline.ToString())
            {
                throw new AppException(ResponseCodeConstants.BAD_REQUEST, ResponseMessageConstrantsOrder.CANT_REFUND_FOR_OFFLINE, StatusCodes.Status400BadRequest);
            }

            var accountId = GetAuthenticatedAccountId();
            var account = await _accountRepo.GetAccountById(accountId);
            if (account.Role != RoleEnums.Manager.ToString())
            {
                throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstrantsMaster.MASTER_NOT_FOUND, StatusCodes.Status404NotFound);
            }

            if (order.Status != PaymentStatusEnums.WaitingForRefund.ToString())
            {
                throw new AppException(ResponseCodeConstants.BAD_REQUEST, ResponseMessageConstrantsOrder.NOT_WAITING_FOR_REFUND, StatusCodes.Status400BadRequest);
            }

            string customerQrUrl = GetCustomerRefundQR(order);
            await UpdateOrderStatusForRefund(order);

            return customerQrUrl;
        }

        private string GetCustomerRefundQR(Order order)
        {
            if (order.Customer?.Account == null)
            {
                throw new AppException(ResponseCodeConstants.BAD_REQUEST, ResponseMessageConstantsUser.CUSTOMER_BANK_INFO_NOT_FOUND, StatusCodes.Status400BadRequest);
            }

            var account = order.Customer.Account;
            if (!account.BankId.HasValue || string.IsNullOrEmpty(account.AccountNo))
            {
                throw new AppException(ResponseCodeConstants.BAD_REQUEST, ResponseMessageConstantsUser.CUSTOMER_BANK_INFO_NOT_FOUND, StatusCodes.Status400BadRequest);
            }
            if (order.Amount <= 0)
            {
                throw new AppException(ResponseCodeConstants.BAD_REQUEST, ResponseMessageConstrantsOrder.ORDER_AMOUNT_INVALID, StatusCodes.Status400BadRequest);
            }

            var parameters = new List<string>
            {
                $"amount={order.Amount}",
                $"addInfo={Uri.EscapeDataString($"Hoàn tiền đơn hàng {order.OrderId}")}"
            };

            return $"https://img.vietqr.io/image/{account.BankId}-{account.AccountNo}-compact.png?{string.Join("&", parameters)}";
        }

        private async Task UpdateOrderStatusForRefund(Order order)
        {
            if(order.Status != PaymentStatusEnums.WaitingForRefund.ToString())
            {
                throw new AppException(ResponseCodeConstants.BAD_REQUEST, ResponseMessageConstrantsOrder.NOT_WAITING_FOR_REFUND, StatusCodes.Status400BadRequest);
            }
            
            order.Status = PaymentStatusEnums.Refunded.ToString();
            await _orderRepo.UpdateOrder(order);
        }
    }
}
