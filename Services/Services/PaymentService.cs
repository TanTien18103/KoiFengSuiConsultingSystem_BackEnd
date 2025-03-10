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
using Repositories.Interfaces;
using Repositories.Repository;

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
        private readonly ICustomerRepo _customerRepo;
        private readonly ICourseRepo _courseRepo;
        private readonly IBookingOnlineRepo _bookingOnlineRepo;
        private readonly IBookingOfflineRepo _bookingOfflineRepo;
        private readonly IWorkShopRepo _workShopRepo;


        public PaymentService(
            IConfiguration configuration, 
            HttpClient httpClient,
            IPriceService priceService,
            IHttpContextAccessor httpContextAccessor,
            ICustomerRepo customerRepo,
            ICourseRepo courseRepo,
            IBookingOnlineRepo bookingOnlineRepo,
            IBookingOfflineRepo bookingOfflineRepo,
            IWorkShopRepo workShopRepo)
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

                // Add service details to orderInfo for consistent response
                string orderInfoJson = JsonSerializer.Serialize(new
                {
                    serviceType = request.PaymentType.ToString(),
                    serviceId = request.ServiceId,
                    customerInfo = new
                    {
                        name = request.CustomerName,
                        email = request.CustomerEmail,
                        phone = request.CustomerPhone
                    }
                });

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
                    orderInfo = orderInfoJson
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
                    var payosResponse = JsonSerializer.Deserialize<PaymentResponse>(responseContent);

                    // Ensure there's a data dictionary for additional info
                    if (payosResponse.Data == null)
                    {
                        payosResponse.Data = new Dictionary<string, object>();
                    }

                    // Add service information to the data dictionary
                    AddServiceDetailsToResponse(payosResponse, request);

                    return payosResponse;
                }

                throw new Exception($"Payment creation failed: {responseContent}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating payment: {ex.Message}");
            }
        }

        private void AddServiceDetailsToResponse(PaymentResponse response, PaymentRequest request)
        {
            if (response.Data is Dictionary<string, object> dataDictionary)
            {
                dataDictionary["serviceType"] = request.PaymentType.ToString();
                dataDictionary["serviceId"] = request.ServiceId;
                dataDictionary["amount"] = request.Amount;
                dataDictionary["description"] = request.Description;
                dataDictionary["customerInfo"] = new
                {
                    name = request.CustomerName,
                    email = request.CustomerEmail,
                    phone = request.CustomerPhone
                };
            }
            else
            {
                // If Data is not a dictionary, initialize it as one
                response.Data = new Dictionary<string, object>
                {
                    ["serviceType"] = request.PaymentType.ToString(),
                    ["serviceId"] = request.ServiceId,
                    ["amount"] = request.Amount,
                    ["description"] = request.Description,
                    ["customerInfo"] = new
                    {
                        name = request.CustomerName,
                        email = request.CustomerEmail,
                        phone = request.CustomerPhone
                    }
                };
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
                    var paymentResponse = JsonSerializer.Deserialize<PaymentResponse>(responseContent);

                    // Extract service details from orderInfo if available
                    if (paymentResponse?.OrderInfo != null)
                    {
                        try
                        {
                            var orderInfo = JsonSerializer.Deserialize<Dictionary<string, object>>(paymentResponse.OrderInfo.ToString());
                            if (orderInfo != null && paymentResponse.Data == null)
                            {
                                paymentResponse.Data = orderInfo;
                            }
                        }
                        catch
                        {
                            // If parsing fails, continue without adding the data
                        }
                    }

                    return paymentResponse;
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
                    var customer = await _customerRepo.GetCustomerById(accountId);

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
                // 1. Tự động điền thông tin khách hàng vào PaymentRequest
                request = await PopulateCustomerInfoForPaymentRequest(request);

                // 2. Xác thực và làm giàu thông tin request
                await EnrichPaymentRequest(request);

                // 3. Xác thực số tiền thanh toán
                await ValidatePaymentAmount(request);

                // 4. Tạo thanh toán 
                return await CreatePaymentAsync(request);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi xử lý thanh toán: {ex.Message}");
            }
        }

        public async Task<PaymentResponse> CreatePaymentForService(PaymentTypeEnums serviceType, string serviceId)
        {
            try
            {
                // Tạo payment request dựa trên loại dịch vụ
                PaymentRequest request = await PreparePaymentRequest(serviceType, serviceId);

                // Populate customer info if not already set
                if (string.IsNullOrEmpty(request.CustomerName) || request.CustomerName == "Khách hàng")
                {
                    request = await PopulateCustomerInfoForPaymentRequest(request);
                }

                // Xác thực và đảm bảo số tiền thanh toán khớp với giá dịch vụ
                var servicePrice = await _priceService.GetServicePrice(request.PaymentType, request.ServiceId);
                if (!servicePrice.HasValue)
                {
                    throw new Exception("Không tìm thấy giá dịch vụ");
                }

                if (request.Amount != servicePrice.Value)
                {
                    // Cập nhật số tiền để khớp với giá dịch vụ
                    request.Amount = servicePrice.Value;
                }

                // Tạo thanh toán với response format thống nhất
                return await CreatePaymentAsync(request);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi xử lý thanh toán: {ex.Message}");
            }
        }

        // Helper method to prepare payment request with consistent structure
        private async Task<PaymentRequest> PreparePaymentRequest(PaymentTypeEnums serviceType, string serviceId)
        {
            PaymentRequest request = new PaymentRequest
            {
                PaymentType = serviceType,
                ServiceId = serviceId,
                ReturnUrl = _configuration["Payment:ReturnUrl"],
                CancelUrl = _configuration["Payment:CancelUrl"],
                CustomerName = "Khách hàng" // Default value
            };

            switch (serviceType)
            {
                case PaymentTypeEnums.BookingOnline:
                    var bookingOnline = await _bookingOnlineRepo.GetBookingOnlineByIdRepo(serviceId);
                    if (bookingOnline == null)
                        throw new Exception("Không tìm thấy đặt phòng online");

                    request.OrderCode = $"BON{bookingOnline.BookingOnlineId}_{DateTime.Now.Ticks}";
                    request.Amount = bookingOnline.Price ?? 0;
                    request.Description = $"Thanh toán đặt phòng online #{bookingOnline.BookingOnlineId}";

                    // Set customer info if available
                    if (bookingOnline.Customer?.Account != null)
                    {
                        request.CustomerName = bookingOnline.Customer.Account.FullName ?? "Khách hàng";
                        request.CustomerEmail = bookingOnline.Customer.Account.Email;
                        request.CustomerPhone = bookingOnline.Customer.Account.PhoneNumber;
                    }
                    break;

                case PaymentTypeEnums.BookingOffline:
                    var bookingOffline = await _bookingOfflineRepo.GetBookingOfflineById(serviceId);
                    if (bookingOffline == null)
                        throw new Exception("Không tìm thấy đặt phòng offline");

                    request.OrderCode = $"BOF{bookingOffline.BookingOfflineId}_{DateTime.Now.Ticks}";
                    request.Amount = bookingOffline.ConsultationPackage?.PackagePrice ?? 0;
                    request.Description = $"Thanh toán đặt phòng offline #{bookingOffline.BookingOfflineId}";

                    // Set customer info if available
                    if (bookingOffline.Customer?.Account != null)
                    {
                        request.CustomerName = bookingOffline.Customer.Account.FullName ?? "Khách hàng";
                        request.CustomerEmail = bookingOffline.Customer.Account.Email;
                        request.CustomerPhone = bookingOffline.Customer.Account.PhoneNumber;
                    }
                    break;

                case PaymentTypeEnums.Course:
                    var course = await _courseRepo.GetCourseById(serviceId);
                    if (course == null)
                        throw new Exception("Không tìm thấy khóa học");

                    request.OrderCode = $"CRS{course.CourseId}_{DateTime.Now.Ticks}";
                    request.Amount = course.Price ?? 0;
                    request.Description = $"Thanh toán khóa học {course.CourseName}";
                    break;

                case PaymentTypeEnums.Workshop:
                    var workshop = await _workShopRepo.GetWorkShopById(serviceId);
                    if (workshop == null)
                        throw new Exception("Không tìm thấy workshop");

                    request.OrderCode = $"WS{workshop.WorkshopId}_{DateTime.Now.Ticks}";
                    request.Amount = workshop.Price ?? 0;
                    request.Description = $"Thanh toán workshop {workshop.WorkshopName}";
                    break;

                default:
                    throw new Exception("Loại dịch vụ không hợp lệ");
            }

            return request;
        }

        private async Task EnrichPaymentRequest(PaymentRequest request)
        {
            // Enrich the payment request with additional information if needed
            // For example, ensure the order code is unique by adding a timestamp if not already present
            if (!request.OrderCode.Contains("_"))
            {
                request.OrderCode = $"{request.OrderCode}_{DateTime.Now.Ticks}";
            }

            // Ensure description is set
            if (string.IsNullOrEmpty(request.Description))
            {
                request.Description = $"Thanh toán dịch vụ {request.PaymentType} #{request.ServiceId}";
            }

            // Set default URLs if not provided
            if (string.IsNullOrEmpty(request.ReturnUrl) || string.IsNullOrEmpty(request.CancelUrl))
            {
                var baseUrl = $"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host}";
                request.ReturnUrl = request.ReturnUrl ?? $"{baseUrl}/api/payment/success";
                request.CancelUrl = request.CancelUrl ?? $"{baseUrl}/api/payment/cancel";
            }
        }

        private async Task ValidatePaymentAmount(PaymentRequest request)
        {
            // Validate that the payment amount matches the service price
            var servicePrice = await _priceService.GetServicePrice(request.PaymentType, request.ServiceId);

            if (!servicePrice.HasValue)
            {
                throw new Exception("Không tìm thấy giá dịch vụ");
            }

            if (request.Amount != servicePrice.Value)
            {
                // Option 1: Throw an exception if amounts don't match
                throw new Exception($"Số tiền thanh toán không khớp. Dự kiến: {servicePrice.Value}, Nhận được: {request.Amount}");

                // Option 2: Update the amount to match the service price (uncomment if preferred)
                // request.Amount = servicePrice.Value;
            }
        }
    }
}
