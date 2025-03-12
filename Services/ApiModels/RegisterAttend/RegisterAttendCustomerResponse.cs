using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.RegisterAttend
{
    public class RegisterAttendCustomerResponse
    {
        public string AttendId { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string WorkshopName { get; set; }
        public DateTime? StartDate { get; set; }
        public string? Location { get; set; }
        public string? MasterName { get; set; }

    }
}
