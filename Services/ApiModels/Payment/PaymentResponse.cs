using System;

namespace BusinessObjects
{
    public class PaymentResponse
    {
        public string Id { get; set; }
        public string OrderCode { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string PaymentUrl { get; set; }
        public object OrderInfo { get; set; }
        public object Data { get; set; }
    }
} 