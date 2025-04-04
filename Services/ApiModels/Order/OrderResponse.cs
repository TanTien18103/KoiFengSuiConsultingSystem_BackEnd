using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Order
{
    public class OrderResponse
    {
        public string OrderId { get; set; }

        public string CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string ServiceId { get; set; }

        public string ServiceType { get; set; }

        public decimal? Amount { get; set; }

        public string OrderCode { get; set; }

        public string PaymentReference { get; set; }

        public string Status { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string PaymentId { get; set; }

        public string Note { get; set; }

        public string Description { get; set; }
    }
}
