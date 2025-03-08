using BusinessObjects;
using BusinessObjects.Models;
using Services.ApiModels.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IPaymentService
    {
        PaymentRequest CreateBookingOnlinePaymentRequest(BookingOnline bookingOnline);
        PaymentRequest CreateBookingOfflinePaymentRequest(BookingOffline bookingOffline);
        PaymentRequest CreateCoursePaymentRequest(Course course, Customer customer = null);
        PaymentRequest CreateWorkshopPaymentRequest(WorkShop workshop, Customer customer = null);
        Task<PaymentResponse> CreatePaymentAsync(PaymentRequest request);
        Task<PaymentResponse> CheckPaymentStatusAsync(string orderId);
        
        // Thêm phương thức mới để tự động điền thông tin khách hàng
        Task<PaymentRequest> PopulateCustomerInfoForPaymentRequest(PaymentRequest request);
        Task<PaymentResponse> ProcessPayment(PaymentRequest request);
    }
}
