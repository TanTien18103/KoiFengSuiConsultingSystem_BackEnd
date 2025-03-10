using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaymentService;

namespace Services.ApiModels.Payment
{
    public class WebhookRequest
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public WebhookData Data { get; set; }
        public string Signature { get; set; }
    }
}
