using BusinessObjects.Models;

namespace Services.ApiModels.MasterSchedule
{
    public class MasterScheduleDTO
    {
        public string MasterScheduleId { get; set; }
        public string MasterId { get; set; }
        public string MasterName { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }
}