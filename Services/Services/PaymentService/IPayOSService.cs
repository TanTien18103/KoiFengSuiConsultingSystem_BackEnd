using Net.payOS.Types;
using Services.ApiModels.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.PaymentService
{
    public interface IPayOSService
    {
        Task<PayOSResponse> CreatePaymentUrl(PayOSRequest request);
    }
}
