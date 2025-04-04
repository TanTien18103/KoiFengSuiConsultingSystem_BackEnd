namespace Services.ApiModels.BookingOnline
{
    public class BookingOnlineDetailResponse
    {
        public string BookingOnlineId { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public string CustomerName { get; set; }

        public string CustomerEmail { get; set; }

        public string MasterName { get; set; }

        public decimal Price { get; set; }

        public string LinkMeet { get; set; }

        public string Description { get; set; }

        public DateOnly? BookingDate { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        public string MasterNote { get; set; }
    }
}
