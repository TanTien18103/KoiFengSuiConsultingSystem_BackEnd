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
        Task<PaymentLinkResponse> CreatePaymentLinkAsync(PaymentTypeEnums serviceType, string serviceId, decimal amount, string customerId, string returnUrl, string cancelUrl);
        Task<bool> ProcessWebhookAsync(WebhookRequest request);
        Task<string> RegisterWebhookUrl();
    }
}
