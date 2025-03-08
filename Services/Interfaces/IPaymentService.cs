using BusinessObjects;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Services.ApiModels.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponse> CreatePaymentForService(PaymentTypeEnums serviceType, string serviceId);
        Task<PaymentRequest> PopulateCustomerInfoForPaymentRequest(PaymentRequest request);
        Task<PaymentResponse> CreatePaymentAsync(PaymentRequest request);
        Task<PaymentResponse> CheckPaymentStatusAsync(string orderId);
    }
}
