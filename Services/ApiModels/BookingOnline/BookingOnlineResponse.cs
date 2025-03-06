using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.BookingOnline
{
    public class BookingOnlineResponse
    {
        public string BookingOnlineId { get; set; }

        public string LinkMeet { get; set; }

        public string Status { get; set; }

        public string MasterId { get; set; }

        public string CustomerId { get; set; }

        public string Description { get; set; }

        public string Customer { get; set; }

        public string Master { get; set; }
    }
}
