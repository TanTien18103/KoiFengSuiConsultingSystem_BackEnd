using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Payment
{
    public class WorkshopRegistration
    {
        public string Id { get; set; }
        public string WorkshopId { get; set; }
        public string CustomerId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentReference { get; set; }
    }
}
