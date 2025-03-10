using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Payment
{
    public class PaymentStatusResponse
    {
        public string OrderCode { get; set; }
        public string Status { get; set; }
        public string TransactionId { get; set; }
        public long Amount { get; set; }
    }
}
