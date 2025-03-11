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

namespace Services.Interfaces
{
    public interface IPaymentService
    {
        Task<CreatePaymentResult> CreatePaymentLinkAsync(PayOSRequest request);
        Task GetWebhookTypeAsync(WebhookType request);
        Task<PaymentLinkInformation> GetPaymentLinkInformationAsync(long orderCode);
        Task ConfirmPayment(string orderId, long orderCode);
        Task<string> ConfirmWebhook(string webhookUrl);
    }
}
