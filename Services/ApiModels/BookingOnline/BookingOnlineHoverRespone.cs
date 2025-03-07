using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.BookingOnline
{
    public class BookingOnlineHoverRespone
    {
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateOnly? BookingDate { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }
        public string LinkMeet { get; set; }
        public string Type { get; set; }

    }
}
