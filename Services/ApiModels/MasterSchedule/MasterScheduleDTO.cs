using BusinessObjects.Models;
using Services.ApiModels.Customer;

namespace Services.ApiModels.MasterSchedule
{
    public class MasterSchedulesListDTO
    {
        public DateOnly? Date { get; set; }
        public List<MasterScheduleDTO> Schedules { get; set; }
    }
    public class MasterScheduleDTO
    {
        public string MasterScheduleId { get; set; }
        public string MasterId { get; set; }
        public string MasterName { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        //public string BookingType { get; set; }
        public List<BookingOnlineDTO> BookingOnlines { get; set; }


        public class BookingOnlineDTO
        {
            public CustomerInfoDTO Customer { get; set; }
        }


        public class CustomerInfoDTO
        {
            public string CustomerId { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
        }
    }
}