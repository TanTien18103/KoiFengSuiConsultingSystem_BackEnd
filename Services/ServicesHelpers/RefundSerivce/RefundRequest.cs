using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServicesHelpers.RefundSerivce
{
    public class RefundRequest
    {
        public string OrderId { get; set; }
        public string CustomerId { get; set; }
    }
}
