using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessObjects.Enums;
using BusinessObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repositories.Interfaces;
using Services.ApiModels.Payment;
using Services.Interfaces;
using System.Security.Cryptography;

public class PaymentService : IPaymentService
{
    private readonly HttpClient _httpClient;
    private readonly string _clientId;
    private readonly string _apiKey;
    private readonly string _checksumKey;
    private readonly string _payosApiUrl;
    private readonly IPriceService _priceService;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICustomerRepo _customerRepo;
    private readonly ICourseRepo _courseRepo;
    private readonly IBookingOnlineRepo _bookingOnlineRepo;
    private readonly IBookingOfflineRepo _bookingOfflineRepo;
    private readonly IWorkShopRepo _workShopRepo;
    private readonly IOrderRepo _orderRepo;

    public PaymentService(
        IConfiguration configuration,
        HttpClient httpClient,
        IPriceService priceService,
        IHttpContextAccessor httpContextAccessor,
        ICustomerRepo customerRepo,
        ICourseRepo courseRepo,
        IBookingOnlineRepo bookingOnlineRepo,
        IBookingOfflineRepo bookingOfflineRepo,
        IWorkShopRepo workShopRepo,
        IOrderRepo orderRepo)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _clientId = configuration["PayOs:ClientId"];
        _apiKey = configuration["PayOs:ApiKey"];
        _checksumKey = configuration["PayOs:ChecksumKey"];
        _payosApiUrl = configuration["PayOs:ApiUrl"];
        _priceService = priceService;
        _httpContextAccessor = httpContextAccessor;
        _customerRepo = customerRepo;
        _courseRepo = courseRepo;
        _bookingOnlineRepo = bookingOnlineRepo;
        _bookingOfflineRepo = bookingOfflineRepo;
        _workShopRepo = workShopRepo;
        _orderRepo = orderRepo;
    }

    public async Task<PaymentLinkResponse> CreatePaymentLinkAsync(PaymentTypeEnums serviceType, string serviceId, decimal amount, string customerId, string returnUrl, string cancelUrl)
    {
        try
        {
            // Tạo mã đơn hàng
            string prefix = GetServicePrefix(serviceType);
            string orderCode = $"{prefix}{serviceId}_{DateTime.Now.Ticks}";

            // Tạo Order mới
            var order = new Order
            {
                CustomerId = customerId,
                ServiceId = serviceId,
                ServiceType = serviceType,
                Amount = amount,
                OrderCode = orderCode,
                Status = PaymentStatusEnums.Pending,
                CreatedDate = DateTime.Now,
                Description = $"Thanh toán cho {serviceType} - {serviceId}",
                ReturnUrl = returnUrl,
                CancelUrl = cancelUrl
            };

            await _orderRepo.CreateOrderAsync(order);

            // Tạo các thông tin cho PayOS
            var baseUrl = GetBaseUrl();
            var paymentData = new
            {
                orderCode = orderCode,
                amount = (int)(amount * 100), // Chuyển đổi sang đơn vị nhỏ nhất (ví dụ: đồng thay vì nghìn đồng)
                description = order.Description,
                returnUrl = returnUrl,
                cancelUrl = cancelUrl,
                // Các thông tin khác theo yêu cầu của PayOS
            };

            // Gọi API của PayOS để tạo payment link
            var content = new StringContent(
                JsonSerializer.Serialize(paymentData),
                Encoding.UTF8,
                "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-client-id", _clientId);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);

            var response = await _httpClient.PostAsync($"{_payosApiUrl}/v2/payment-requests", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Lỗi tạo payment link: {responseContent}");
            }

            var paymentLinkResponse = JsonSerializer.Deserialize<PaymentLinkResponse>(responseContent);

            // Cập nhật PaymentUrl vào Order
            order.PaymentUrl = paymentLinkResponse.CheckoutUrl;
            await _orderRepo.UpdateOrderAsync(order);

            return paymentLinkResponse;
        }
        catch (Exception ex)
        {
            throw new Exception($"Không thể tạo link thanh toán: {ex.Message}", ex);
        }
    }

    private string GetServicePrefix(PaymentTypeEnums serviceType)
    {
        switch (serviceType)
        {
            case PaymentTypeEnums.BookingOnline:
                return "BON";
            case PaymentTypeEnums.BookingOffline:
                return "BOF";
            case PaymentTypeEnums.Course:
                return "CRS";
            case PaymentTypeEnums.Workshop:
                return "WS";
            default:
                throw new ArgumentException($"Invalid service type: {serviceType}");
        }
    }

    public async Task<bool> ProcessWebhookAsync(WebhookRequest request)
    {
        try
        {
            // Kiểm tra tính hợp lệ của webhook
            if (request == null || request.Data == null)
            {
                Console.WriteLine("Webhook request data is null");
                return false;
            }

            // Xác minh chữ ký webhook
            bool isValid = VerifyWebhookSignature(request);
            if (!isValid)
            {
                Console.WriteLine("Invalid webhook signature");
                return false;
            }

            // Lấy mã đơn hàng
            var orderCode = request.Data.OrderCode;
            Console.WriteLine($"Processing webhook for order code: {orderCode}");

            // Kiểm tra trạng thái thanh toán
            if (request.Code == "00") // Mã thành công từ PayOS
            {
                // Tìm order tương ứng với orderCode
                var order = await _orderRepo.GetOrderByOrderCodeAsync(orderCode);
                if (order == null)
                {
                    Console.WriteLine($"Order not found for code: {orderCode}");
                    return false;
                }

                // Cập nhật thông tin thanh toán
                order.Status = PaymentStatusEnums.Paid;
                order.PaymentDate = DateTime.Now;
                order.PaymentReference = orderCode;

                await _orderRepo.UpdateOrderAsync(order);

                // Cập nhật trạng thái trong bảng dịch vụ tương ứng
                await UpdateServicePaymentStatusAsync(order);

                return true;
            }
            else
            {
                Console.WriteLine($"Payment failed with code: {request.Code} for order: {orderCode}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing webhook: {ex.Message}");
            return false;
        }
    }

    // Phương thức xác minh chữ ký webhook
    private bool VerifyWebhookSignature(WebhookRequest request)
    {
        try
        {
            // Lấy chữ ký từ request
            string signature = request.Signature;

            // Tạo chữ ký từ dữ liệu
            string dataJson = JsonSerializer.Serialize(request.Data);
            string rawSignature = $"{_checksumKey}{dataJson}";

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_checksumKey)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawSignature));
                string computedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();

                // So sánh chữ ký
                return signature.Equals(computedSignature, StringComparison.OrdinalIgnoreCase);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error verifying webhook signature: {ex.Message}");
            return false;
        }
    }

    // Phương thức cập nhật trạng thái thanh toán trong bảng dịch vụ
    private async Task<bool> UpdateServicePaymentStatusAsync(Order order)
    {
        try
        {
            switch (order.ServiceType)
            {
                case PaymentTypeEnums.BookingOnline:
                    var bookingOnline = await _bookingOnlineRepo.GetBookingOnlineByIdRepo(order.ServiceId);
                    if (bookingOnline != null)
                    {
                        bookingOnline.Status = PaymentStatusEnums.Paid.ToString();
                        bookingOnline.PaymentReference = order.PaymentReference;
                        bookingOnline.PaymentDate = order.PaymentDate;
                        await _bookingOnlineRepo.UpdateBookingOnlineRepo(bookingOnline);
                        return true;
                    }
                    break;

                case PaymentTypeEnums.BookingOffline:
                    var bookingOffline = await _bookingOfflineRepo.GetBookingOfflineById(order.ServiceId);
                    if (bookingOffline != null)
                    {
                        bookingOffline.Status = PaymentStatusEnums.Paid.ToString();
                        bookingOffline.PaymentReference = order.PaymentReference;
                        bookingOffline.PaymentDate = order.PaymentDate;
                        await _bookingOfflineRepo.UpdateBookingOffline(bookingOffline);
                        return true;
                    }
                    break;

                case PaymentTypeEnums.Course:
                    // Cập nhật trạng thái đăng ký khóa học
                    var courseRegistration = new CourseRegistration
                    {
                        CourseId = order.ServiceId,
                        CustomerId = order.CustomerId,
                        RegistrationDate = DateTime.Now,
                        PaymentStatus = "Paid",
                        PaymentReference = order.PaymentReference
                    };
                    // Lưu vào cơ sở dữ liệu
                    // await _courseRegistrationRepo.Add(courseRegistration);
                    return true;

                case PaymentTypeEnums.Workshop:
                    // Tương tự như với Course
                    var workshopRegistration = new WorkshopRegistration
                    {
                        WorkshopId = order.ServiceId,
                        CustomerId = order.CustomerId,
                        RegistrationDate = DateTime.Now,
                        PaymentStatus = "Paid",
                        PaymentReference = order.PaymentReference
                    };
                    // await _workshopRegistrationRepo.Add(workshopRegistration);
                    return true;

                default:
                    Console.WriteLine($"Unknown service type: {order.ServiceType}");
                    return false;
            }

            Console.WriteLine($"Service not found - Type: {order.ServiceType}, ID: {order.ServiceId}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating service payment status: {ex.Message}");
            return false;
        }
    }

    public async Task<string> RegisterWebhookUrl()
    {
        try
        {
            var baseUrl = GetBaseUrl();
            var webhookUrl = $"{baseUrl}/api/payment/webhook";

            var data = new
            {
                webhookUrl = webhookUrl
            };

            var content = new StringContent(
                JsonSerializer.Serialize(data),
                Encoding.UTF8,
                "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-client-id", _clientId);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);

            var response = await _httpClient.PostAsync($"{_payosApiUrl}/v2/webhooks", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Lỗi đăng ký webhook: {responseContent}");
            }

            return webhookUrl;
        }
        catch (Exception ex)
        {
            throw new Exception($"Không thể đăng ký webhook: {ex.Message}", ex);
        }
    }

    // Helper method to get base URL
    private string GetBaseUrl()
    {
        var request = _httpContextAccessor.HttpContext.Request;
        return $"{request.Scheme}://{request.Host}";
    }

    // Phương thức lấy thông tin khách hàng hiện tại
    private async Task<object> GetCurrentCustomerInfo()
    {
        // Implement method to get current customer info
        // ...
        return null;
    }

    // Phương thức lấy ID của khách hàng hiện tại
    private async Task<string> GetCurrentCustomerId()
    {
        var customerInfo = await GetCurrentCustomerInfo();
        if (customerInfo != null && customerInfo is Dictionary<string, object> customerDict)
        {
            if (customerDict.TryGetValue("id", out var idObj))
                return idObj?.ToString();
        }
        return null;
    }
}