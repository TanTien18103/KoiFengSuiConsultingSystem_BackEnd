using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.Dto
{
    public class MonthlyServiceStatisticsDto
    {
        public string MonthYear { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Courses { get; set; }
        public int Workshops { get; set; }
        public int BookingOnline { get; set; }
        public int BookingOffline { get; set; }
    }
}
