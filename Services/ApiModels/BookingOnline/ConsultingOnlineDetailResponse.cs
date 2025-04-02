using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.BookingOnline
{
    public class ConsultingOnlineDetailResponse
    {
        public string ConsultingId { get; set; }

        public string Type { get; set; }

        public string CustomerName { get; set; }

        public string Description { get; set; }

        public DateOnly? BookingDate { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        public string MasterNote { get; set; }
    }
}
