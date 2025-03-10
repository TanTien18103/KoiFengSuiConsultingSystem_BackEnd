using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Payment
{
    public class WebhookData
    {
        public string OrderCode { get; set; }
        public long Amount { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string TransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
