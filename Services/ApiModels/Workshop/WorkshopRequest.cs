using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Workshop
{
    public class WorkshopRequest
    {
        public string WorkshopName { get; set; }
        public DateTime? StartDate { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public int? Capacity { get; set; }
        public decimal? Price { get; set; }
    }
}
