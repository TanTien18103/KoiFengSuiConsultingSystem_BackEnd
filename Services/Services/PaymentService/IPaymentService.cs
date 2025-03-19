using BusinessObjects;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Net.payOS.Types;
using Services.ApiModels;
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
    }
}
