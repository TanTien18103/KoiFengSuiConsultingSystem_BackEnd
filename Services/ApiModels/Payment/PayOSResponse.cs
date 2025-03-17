using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Payment
{
    public class PayOSResponse
    {
        public string OrderId { get; set; }
        public string Status { get; set; }
        public string CheckoutUrl { get; set; }
        public string QrCode { get; set; }
        public PaymentLinkInformation Data { get; set; }
    }
}
