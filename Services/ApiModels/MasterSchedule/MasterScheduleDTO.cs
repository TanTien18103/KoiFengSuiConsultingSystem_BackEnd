using BusinessObjects.Models;
using DAOs.DAOs;
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
        public string? Location { get; set; }
        public List<BookingOnlineDTO> BookingOnlines { get; set; }
        public List<BookingOfflineDTO> BookingOfflines { get; set; }
        public List<WorkshopDTO> Workshops { get; set; }

        public class BookingOfflineDTO
        {
            public string BookingOfflineId { get; set; }
            public CustomerInfoDTO Customer { get; set; }
            public string Location { get; set; }
        }

        public class BookingOnlineDTO
        {
            public string BookingOnlineId { get; set; }
            public CustomerInfoDTO Customer { get; set; }
        }
        public class WorkshopDTO
        {
            public string WorkshopId { get; set; }
            public string WorkshopName { get; set; }
            public string LocationId { get; set; }
            public string LocationName { get; set; }
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