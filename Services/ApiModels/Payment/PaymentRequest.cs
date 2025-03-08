using BusinessObjects.Enums;

namespace Services.ApiModels.Payment
{
    public class PaymentRequest
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public PaymentTypeEnums PaymentType { get; set; }
        public int ServiceId { get; set; } // ID của BookingOnline/BookingOffline/Course/Workshop
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
    }
} 