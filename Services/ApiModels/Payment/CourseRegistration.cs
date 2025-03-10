using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Payment
{
    public class CourseRegistration
    {
        public string Id { get; set; }
        public string CourseId { get; set; }
        public string CustomerId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentReference { get; set; }
    }
}
