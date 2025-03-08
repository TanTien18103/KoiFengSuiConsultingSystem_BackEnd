using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessObjects;
using BusinessObjects.Models;
using BusinessObjects.Enums;
using Microsoft.Extensions.Configuration;
using Services.ApiModels.Payment;
using Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Services.Services
{
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
        private readonly ICustomerService _customerService;

        public PaymentService(
            IConfiguration configuration, 
            HttpClient httpClient,
            IPriceService priceService,
            IHttpContextAccessor httpContextAccessor,
            ICustomerService customerService)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _clientId = configuration["PayOs:ClientId"];
            _apiKey = configuration["PayOs:ApiKey"];
            _checksumKey = configuration["PayOs:ChecksumKey"];
            _payosApiUrl = configuration["PayOs:ApiUrl"];
            _priceService = priceService;
            _httpContextAccessor = httpContextAccessor;
            _customerService = customerService;
        }

        public PaymentRequest CreateBookingOnlinePaymentRequest(BookingOnline bookingOnline)
        {
            string customerName = "Khách hàng";
            if (bookingOnline.Customer?.Account?.FullName != null)
            {
                customerName = bookingOnline.Customer.Account.FullName;
            }

            return new PaymentRequest
            {
                OrderCode = $"BON{bookingOnline.BookingOnlineId}",
                Amount = bookingOnline.Price ?? 0,
                Description = $"Thanh toán đặt phòng online #{bookingOnline.BookingOnlineId}",
                ReturnUrl = _configuration["Payment:ReturnUrl"],
                CancelUrl = _configuration["Payment:CancelUrl"],
                CustomerName = customerName,
                CustomerEmail = bookingOnline.Customer?.Account?.Email,
                CustomerPhone = bookingOnline.Customer?.Account?.PhoneNumber,
                PaymentType = PaymentTypeEnums.BookingOnline,
                ServiceId = bookingOnline.BookingOnlineId
            };
        }

        public PaymentRequest CreateBookingOfflinePaymentRequest(BookingOffline bookingOffline)
        {
            string customerName = "Khách hàng";
            if (bookingOffline.Customer?.Account?.FullName != null)
            {
                customerName = bookingOffline.Customer.Account.FullName;
            }

            return new PaymentRequest
            {
                OrderCode = $"BOF{bookingOffline.BookingOfflineId}",
                Amount = bookingOffline.ConsultationPackage?.PackagePrice ?? 0,
                Description = $"Thanh toán đặt phòng offline #{bookingOffline.BookingOfflineId}",
                ReturnUrl = _configuration["Payment:ReturnUrl"],
                CancelUrl = _configuration["Payment:CancelUrl"],
                CustomerName = customerName,
                CustomerEmail = bookingOffline.Customer?.Account?.Email,
                CustomerPhone = bookingOffline.Customer?.Account?.PhoneNumber,
                PaymentType = PaymentTypeEnums.BookingOffline,
                ServiceId = bookingOffline.BookingOfflineId
            };
        }

        public PaymentRequest CreateCoursePaymentRequest(Course course, Customer customer = null)
        {
            string customerName = "Khách hàng";
            string customerEmail = null;
            string customerPhone = null;

            if (customer?.Account != null)
            {
                customerName = customer.Account.FullName ?? customerName;
                customerEmail = customer.Account.Email;
                customerPhone = customer.Account.PhoneNumber;
            }

            return new PaymentRequest
            {
                OrderCode = $"CRS{course.CourseId}",
                Amount = course.Price ?? 0,
                Description = $"Thanh toán khóa học {course.CourseName}",
                ReturnUrl = _configuration["Payment:ReturnUrl"],
                CancelUrl = _configuration["Payment:CancelUrl"],
                CustomerName = customerName,
                CustomerEmail = customerEmail,
                CustomerPhone = customerPhone,
                PaymentType = PaymentTypeEnums.Course,
                ServiceId = course.CourseId
            };
        }

        public PaymentRequest CreateWorkshopPaymentRequest(WorkShop workshop, Customer customer = null)
        {
            string customerName = "Khách hàng";
            string customerEmail = null;
            string customerPhone = null;

            if (customer?.Account != null)
            {
                customerName = customer.Account.FullName ?? customerName;
                customerEmail = customer.Account.Email;
                customerPhone = customer.Account.PhoneNumber;
            }

            return new PaymentRequest
            {
                OrderCode = $"WS{workshop.WorkshopId}",
                Amount = workshop.Price ?? 0,
                Description = $"Thanh toán workshop {workshop.WorkshopName}",
                ReturnUrl = _configuration["Payment:ReturnUrl"],
                CancelUrl = _configuration["Payment:CancelUrl"],
                CustomerName = customerName,
                CustomerEmail = customerEmail,
                CustomerPhone = customerPhone,
                PaymentType = PaymentTypeEnums.Workshop,
                ServiceId = workshop.WorkshopId
            };
        }

        public async Task<PaymentResponse> CreatePaymentAsync(PaymentRequest request)
        {
            try
            {
                // Validate and get price from service
                var servicePrice = await _priceService.GetServicePrice(request.PaymentType, request.ServiceId);
                if (!servicePrice.HasValue)
                {
                    throw new Exception("Service price not found");
                }

                // Validate that the requested amount matches the service price
                if (request.Amount != servicePrice.Value)
                {
                    throw new Exception($"Invalid payment amount. Expected: {servicePrice.Value}, Received: {request.Amount}");
                }

                var paymentData = new
                {
                    orderCode = request.OrderCode,
                    amount = request.Amount,
                    description = request.Description,
                    cancelUrl = request.CancelUrl,
                    returnUrl = request.ReturnUrl,
                    signature = GenerateSignature(request),
                    customerName = request.CustomerName,
                    customerEmail = request.CustomerEmail,
                    customerPhone = request.CustomerPhone,
                    orderInfo = $"{request.PaymentType}_{request.ServiceId}"
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(paymentData),
                    Encoding.UTF8,
                    "application/json");

                // Clear existing headers before adding new ones
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-client-id", _clientId);
                _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);

                var response = await _httpClient.PostAsync($"{_payosApiUrl}/v2/payment-requests", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<PaymentResponse>(responseContent);
                }

                throw new Exception($"Payment creation failed: {responseContent}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating payment: {ex.Message}");
            }
        }

        public async Task<PaymentResponse> CheckPaymentStatusAsync(string orderId)
        {
            try
            {
                // Clear existing headers before adding new ones
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("x-client-id", _clientId);
                _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);

                var response = await _httpClient.GetAsync($"{_payosApiUrl}/v2/payment-requests/{orderId}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<PaymentResponse>(responseContent);
                }

                throw new Exception($"Payment status check failed: {responseContent}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking payment status: {ex.Message}");
            }
        }

        private string GenerateSignature(PaymentRequest request)
        {
            // Implement signature generation according to PayOs documentation
            var dataToSign = $"{request.OrderCode}|{request.Amount}|{_checksumKey}";
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        public async Task<PaymentRequest> PopulateCustomerInfoForPaymentRequest(PaymentRequest request)
        {
            // Lấy thông tin khách hàng đang đăng nhập
            var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            if (identity != null && identity.IsAuthenticated)
            {
                var claims = identity.Claims;
                var accountId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                
                if (!string.IsNullOrEmpty(accountId))
                {
                    // Lấy thông tin customer từ accountId
                    var customer = await _customerService.GetCustomerById(accountId);
                    
                    if (customer != null && customer.Account != null)
                    {
                        // Tự động điền thông tin khách hàng vào PaymentRequest
                        request.CustomerName = customer.Account.FullName ?? "Khách hàng";
                        request.CustomerEmail = customer.Account.Email;
                        request.CustomerPhone = customer.Account.PhoneNumber;
                    }
                }
            }
            
            // Tạo OrderId duy nhất nếu chưa có
            if (string.IsNullOrEmpty(request.OrderCode))
            {
                request.OrderCode = $"{request.PaymentType}_{request.ServiceId}_{DateTime.Now.Ticks}";
            }
            
            // Thiết lập URL callback nếu chưa có
            if (string.IsNullOrEmpty(request.ReturnUrl) || string.IsNullOrEmpty(request.CancelUrl))
            {
                var baseUrl = $"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host}";
                request.ReturnUrl = request.ReturnUrl ?? $"{baseUrl}/api/payment/success";
                request.CancelUrl = request.CancelUrl ?? $"{baseUrl}/api/payment/cancel";
            }
            
            return request;
        }

        public async Task<PaymentResponse> ProcessPayment(PaymentRequest request)
        {
            try
            {
                // 1. Tự động điền thông tin khách hàng vào PaymentRequest (logic từ Controller)
                request = await PopulateCustomerInfoForPaymentRequest(request);
                
                // 2. Xác thực và làm giàu thông tin request
                //await EnrichPaymentRequest(request);
                
                //// 3. Xác thực số tiền thanh toán
                //await ValidatePaymentAmount(request);
                
                // 4. Tạo thanh toán (logic từ Controller)
                return await CreatePaymentAsync(request);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi xử lý thanh toán: {ex.Message}");
            }
        }
    }
} 