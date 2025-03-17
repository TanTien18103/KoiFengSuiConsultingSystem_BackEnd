using BusinessObjects;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Net.payOS.Types;
using Services.ApiModels.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.PaymentService
{
    public interface IPaymentService
    {
        Task<ResultModel> Payment(string serviceId, PaymentTypeEnums serviceType);
        Task<CreatePaymentResult> CreateServicePaymentLinkAsync(PaymentTypeEnums serviceType, string serviceId, string cancelUrl, string returnUrl);
        Task GetWebhookTypeAsync(WebhookType request);
        Task<PaymentLinkInformation> GetPaymentLinkInformationAsync(long orderCode);
        Task ConfirmPayment(string orderId, long orderCode);
        Task<string> ConfirmWebhook(string webhookUrl);
    }
}
