using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.BookingOffline
{
    public class BookingOfflineResponse
    {
        public string BookingOfflineId { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public string CustomerName { get; set; }

        public string MasterName { get; set; }
        public string Location { get; set; }

        public string Description { get; set; }

        public DateOnly? BookingDate { get; set; }

        public string MasterNote { get; set; }
    }
}
