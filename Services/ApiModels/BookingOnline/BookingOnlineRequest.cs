using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.BookingOnline
{
    public class BookingOnlineRequest
    {
        public string? MasterId { get; set; } 
        public string Description { get; set; }
        public DateOnly? BookingDate { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }


    }
}
