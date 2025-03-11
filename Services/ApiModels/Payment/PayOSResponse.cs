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
        public String? Code { get; set; }
        public String? Desc { get; set; }
        public PaymentLinkInformation? Data { get; set; }
        public String? Signature { get; set; }
    }
}
