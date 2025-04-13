using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.Workshop
{
    public class WorkshopResponse
    {
        public string WorkshopId { get; set; } = null!;
        public string? WorkshopName { get; set; }
        public DateTime? StartDate { get; set; }
        public string? Location { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public string? Description { get; set; }
        public string ImageUrl { get; set; }
        public int? Capacity { get; set; }
        public string? Status { get; set; }
        public decimal? Price { get; set; }
        public string MasterId { get; set; }
        public string? MasterName { get; set; }
    }
}
