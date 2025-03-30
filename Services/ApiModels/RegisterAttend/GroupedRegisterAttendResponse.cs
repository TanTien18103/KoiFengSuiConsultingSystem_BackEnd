using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.RegisterAttend
{
    public class GroupedRegisterAttendResponse
    {
        public string GroupId { get; set; }
        public string WorkshopId { get; set; }
        public string WorkshopName { get; set; }
        public string Status { get; set; }
        public int NumberOfTickets { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
    }
}
