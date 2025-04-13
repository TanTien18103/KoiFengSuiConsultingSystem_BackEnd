using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.MasterSchedule
{
    public class MasterSchedulesForMobileResponse
    {
        public string MasterScheduleId { get; set; }

        public string MasterId { get; set; }

        public DateOnly? Date { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        public string Type { get; set; }
    }
}
