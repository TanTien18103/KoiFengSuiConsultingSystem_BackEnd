namespace Services.ApiModels.BookingOffline
{
    public class BookingOfflineDetailResponse
    {
        public string BookingOfflineId { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public string CustomerName { get; set; }

        public string PackageName { get; set; }

        public string CustomerEmail { get; set; }

        public string MasterName { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public decimal? SelectedPrice { get; set; }

        public DateOnly? BookingDate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
