using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels
{
    public class DashboardResponse
    {
        public decimal Revenue { get; set; }
        public int TotalCustomers { get; set; }

        public GenderStats GenderStats { get; set; }

        public List<MonthlyComparison> MonthlyComparison { get; set; }

        public List<HourlyAdmit> TimeAdmittedToday { get; set; }

        public TodayRecord TodayRecord { get; set; }
    }

    public class GenderStats
    {
        public int Male { get; set; }
        public int Female { get; set; }
    }

    public class MonthlyComparison
    {
        public string MonthYear { get; set; }
        public int Courses { get; set; }
        public int Workshops { get; set; }
        public int BookingOnline { get; set; }
        public int BookingOffline { get; set; }

    }

    public class HourlyAdmit
    {
        public string Time { get; set; }
        public int Count { get; set; }
    }

    public class TodayRecord
    {
        public int Courses { get; set; }
        public int CheckInWorkshops { get; set; }
        public int BookingOnline { get; set; }
        public int BookingOffline { get; set; }
    }

}
