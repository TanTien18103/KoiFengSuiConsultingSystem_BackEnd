using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessObjects.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Services.ApiModels.Payment;
using System.Security.Cryptography;
using BusinessObjects.Models;
using Repositories.Repositories;
using Net.payOS;
using Net.payOS.Types;
using Newtonsoft.Json;
using WebhookData = Net.payOS.Types.WebhookData;
using Services.ServicesHelpers.PriceService;
using Repositories.Repositories.AccountRepository;
using Repositories.Repositories.BookingOfflineRepository;
using Repositories.Repositories.BookingOnlineRepository;
using Repositories.Repositories.CourseRepository;
using Repositories.Repositories.CustomerRepository;
using Repositories.Repositories.OrderRepository;
using Repositories.Repositories.WorkShopRepository;
using BusinessObjects.Constants;
using BusinessObjects.Exceptions;
using static BusinessObjects.Constants.ResponseMessageConstrantsKoiPond;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Services.ApiModels;
using Repositories.Repositories.RegisterAttendRepository;
using AutoMapper;

namespace Services.Services.PaymentService
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
        private readonly IOrderRepo _orderRepo;
        private readonly IAccountRepo _accountRepo;
        private readonly IPayOSService _payOSService;
        private readonly IRegisterAttendRepo _registerAttendRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentService> _logger;

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
            IOrderRepo orderRepo,
            IAccountRepo accountRepo,
            IPayOSService payOSService,
            IRegisterAttendRepo registerAttendRepo,
            IMapper mapper,
            ILogger<PaymentService> logger)
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
            _accountRepo = accountRepo;
            _payOSService = payOSService;
            _registerAttendRepo = registerAttendRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResultModel> Payment(string serviceId, PaymentTypeEnums serviceType)
        {
            var res = new ResultModel();
            try
            {
                var curCustomer = await GetCurrentCustomer();
                if (curCustomer == null)
                {
                    throw new AppException(ResponseCodeConstants.UNAUTHORIZED, "Unauthorized", StatusCodes.Status401Unauthorized);
                }

                // Thêm kiểm tra thanh toán pending
                await CheckPendingPayments(curCustomer.CustomerId, serviceType);

                string cancelUrl = "https://yourdomain.com/cancel";
                string returnUrl = "https://yourdomain.com/return";
                string description = "";
                string customerName = "";
                decimal price = 0;

                switch (serviceType)
                {
                    case PaymentTypeEnums.RegisterAttend:
                        // Lấy danh sách RegisterAttend theo GroupId
                        var registerAttends = await _registerAttendRepo.GetRegisterAttendsByGroupId(serviceId);
                        if (!registerAttends.Any())
                            throw new AppException(ResponseCodeConstants.NOT_FOUND, "Không tìm thấy thông tin đăng ký", StatusCodes.Status404NotFound);
                        // Lấy thông tin workshop
                        var workshop = await _workShopRepo.GetWorkShopById(registerAttends.First().WorkshopId);
                        if (workshop == null)
                            throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstrantsWorkshop.WORKSHOP_NOT_FOUND, StatusCodes.Status404NotFound);
                        // Kiểm tra workshop đã bắt đầu chưa
                        if (workshop.StartDate <= DateTime.Now)
                            throw new AppException(ResponseCodeConstants.BAD_REQUEST, "Workshop đã bắt đầu, không thể thanh toán", StatusCodes.Status400BadRequest);

                        var totalTickets = registerAttends.Count;
                        var totalAmount = workshop.Price * totalTickets;

                        description = "Thanh toán vé tham dự";
                        customerName = curCustomer.Account.FullName ?? "Khách hàng";
                        price = (decimal)totalAmount;
                        break;

                    case PaymentTypeEnums.BookingOnline:
                        var bookingOnline = await _bookingOnlineRepo.GetBookingOnlineByIdRepo(serviceId);
                        if (bookingOnline.BookingDate <= DateOnly.FromDateTime(DateTime.Now) && bookingOnline.StartTime <= TimeOnly.FromDateTime(DateTime.Now))
                            throw new AppException(ResponseCodeConstants.BAD_REQUEST, "Buổi tư vấn đã bắt đầu, không thể thanh toán", StatusCodes.Status400BadRequest);
                        if (bookingOnline == null)
                            throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstrantsBooking.NOT_FOUND_ONLINE, StatusCodes.Status404NotFound);

                        description = "Thanh toán đặt lịch trực tuyến";
                        customerName = curCustomer.Account.FullName ?? "Khách hàng";
                        price = await _priceService.GetServicePrice(serviceType, serviceId) ?? 0;
                        break;

                    case PaymentTypeEnums.BookingOffline:
                        var bookingOffline = await _bookingOfflineRepo.GetBookingOfflineById(serviceId);
                        if (bookingOffline == null)
                            throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstrantsBooking.NOT_FOUND_OFFLINE, StatusCodes.Status404NotFound);

                        description = "Thanh toán gói tư vấn";
                        customerName = curCustomer.Account.FullName ?? "Khách hàng";
                        price = await _priceService.GetServicePrice(serviceType, serviceId) ?? 0;
                        break;

                    case PaymentTypeEnums.Course:
                        var course = await _courseRepo.GetCourseById(serviceId);
                        if (course == null)
                            throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstrantsCourse.COURSE_NOT_FOUND, StatusCodes.Status404NotFound);

                        description = "Thanh toán khóa học";
                        customerName = curCustomer.Account.FullName ?? "Khách hàng";
                        price = await _priceService.GetServicePrice(serviceType, serviceId) ?? 0;
                        break;

                    default:
                        throw new AppException(ResponseCodeConstants.BAD_REQUEST, ResponseMessageConstrantsOrder.SERVICE_TYPE_INVALID, StatusCodes.Status400BadRequest);
                }

                // Tạo mã đơn hàng tạm thời
                string tempOrderId = GenerateShortGuid();

                var payOS = new PayOS(_clientId, _apiKey, _checksumKey);
                // Tạo item cho thanh toán
                ItemData item = new ItemData(
                    $"{description} cho {customerName}",
                    1,
                    (int)price
                );
                List<ItemData> items = new List<ItemData>();
                items.Add(item);

                // Thời gian hết hạn thanh toán (15 phút)
                var expiredAt = ConvertDateTimeToUnixTimestamp(DateTime.Now.AddMinutes(15));
                int orderCode = int.Parse(DateTime.Now.ToString("ffffff"));

                // Tạo dữ liệu thanh toán với thông tin chi tiết
                PaymentData paymentData = new PaymentData(
                    orderCode,
                    (int)price,
                    description,
                    items,
                    cancelUrl, 
                    returnUrl,
                    expiredAt: expiredAt
                );

                // Tạo chữ ký cho dữ liệu thanh toán
                var signature = CreateSignature(payOS, paymentData);
                paymentData = paymentData with { signature = signature };

                // Tạo liên kết thanh toán
                CreatePaymentResult createPayment = await payOS.createPaymentLink(paymentData);

                // Tạo Order mới với trạng thái Pending
                var order = new Order
                {
                    OrderId = tempOrderId,
                    CustomerId = curCustomer.CustomerId,
                    ServiceId = serviceId,
                    ServiceType = serviceType.ToString(),
                    Amount = price,
                    OrderCode = orderCode.ToString(),
                    PaymentReference = createPayment.checkoutUrl,
                    Status = PaymentStatusEnums.Pending.ToString(),
                    CreatedDate = DateTime.Now,
                    Description = description,
                    PaymentId = GenerateShortGuid(),
                    Note = $"Thanh toán cho {description}"
                };

                // Lưu Order vào database
                await _orderRepo.CreateOrder(order);

                // Trả về kết quả link thanh toán
                res.IsSuccess = true;
                res.ResponseCode = ResponseCodeConstants.SUCCESS;
                res.StatusCode = StatusCodes.Status200OK;
                res.Data = new { PaymentUrl = createPayment.checkoutUrl };
                res.Message = "Tạo URL thanh toán thành công";
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccess = false;
                res.ResponseCode = ResponseCodeConstants.FAILED;
                res.StatusCode = StatusCodes.Status500InternalServerError;
                res.Message = ex.Message;
                return res;
            }
        }

        private string CreateSignature(PayOS payOs, PaymentData paymentData)
        {
            // Prepare data by extracting properties from the PaymentData object, sorted by property name
            var properties = paymentData.GetType().GetProperties()
                .OrderBy(p => p.Name)
                // Ensure value is not null
                .Where(p => p.GetValue(paymentData) != null);

            StringBuilder sb = new StringBuilder();

            // Build the query string (key1=value1&key2=value2...)
            foreach (var property in properties)
            {
                var value = property.GetValue(paymentData)?.ToString();
                // Skip if the value is an empty string
                if (!string.IsNullOrEmpty(value))
                {
                    sb.Append($"{property.Name}={value}&");
                }
            }

            // Remove the last '&' if it exists
            if (sb.Length > 0)
            {
                // Efficient way to remove the trailing '&'
                sb.Length--;
            }

            // Encrypt the resulting string using HMAC-SHA256
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_checksumKey)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
                // Convert hash to lowercase hexadecimal
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        public async Task GetWebhookTypeAsync(WebhookType request)
        {
            var orderCode = request.data.orderCode;
            var payOs = new PayOS(_clientId, _apiKey, _checksumKey);
            WebhookData data = payOs.verifyPaymentWebhookData(request);
            if (data == null)
            {
                throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstrantsOrder.WEBHOOK_NOT_FOUND, StatusCodes.Status404NotFound);
            }

            var orderId = request.data.description.Split(" ").Last();

            // valid data & change status of order
            var order = await _orderRepo.GetOrderById(orderId);
            if (order == null)
            {
                throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstrantsOrder.NOT_FOUND + orderId, StatusCodes.Status404NotFound);
            }

            // if status == "00" change status of order to success
            if (request.code == "00")
            {
                await UpdateOrderPayment(order, data.orderCode);
            }
            else
            {
                throw new AppException(ResponseCodeConstants.BAD_REQUEST, ResponseMessageConstrantsOrder.REQUEST_FAILED_ORDER + orderId, StatusCodes.Status400BadRequest);
            }
        }

        public async Task<PaymentLinkInformation> GetPaymentLinkInformationAsync(long orderCode)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-client-id", _clientId);
            client.DefaultRequestHeaders.Add("x-api-key", _apiKey);
            var response = await client.GetAsync($"https://api-merchant.payos.vn/v2/payment-requests/{orderCode}");
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                var payOsResponse = JsonConvert.DeserializeObject<PayOSResponse>(content);
                return payOsResponse.Data;
            }
            throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstrantsOrder.NEED_TO_PAY_SERVICE_NOT_FOUND, StatusCodes.Status404NotFound);
        }

        public async Task ConfirmPayment(string orderId, long orderCode)
        {
            var order = await _orderRepo.GetOrderById(orderId);
            if (order.Status == PaymentStatusEnums.Paid.ToString())
            {
                throw new AppException(ResponseCodeConstants.BAD_REQUEST, ResponseMessageConstrantsOrder.ALREADY_PAID, StatusCodes.Status400BadRequest);
            }

            var payment = await GetPaymentLinkInformationAsync(orderCode);
            if (payment == null)
            {
                throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstrantsOrder.NEED_TO_PAY_SERVICE_NOT_FOUND, StatusCodes.Status404NotFound);
            }

            if (payment.status != "PAID")
            {
                throw new AppException(ResponseCodeConstants.BAD_REQUEST, ResponseMessageConstrantsOrder.NOT_PAID, StatusCodes.Status400BadRequest);
            }
            await UpdateOrderPayment(order, orderCode);
        }

        private async Task UpdateOrderPayment(Order order, long orderCode)
        {
            order.Status = PaymentStatusEnums.Paid.ToString();
            order.PaymentDate = DateTime.Now;
            order.PaymentId = orderCode.ToString();
            await _orderRepo.UpdateOrder(order);
        }

        public async Task<string> ConfirmWebhook(string webhookUrl)
        {
            var payOs = new PayOS(_clientId, _apiKey, _checksumKey);
            return await payOs.confirmWebhook(webhookUrl);
        }

        private string GetAuthenticatedAccountId()
        {
            var identity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
            if (identity == null || !identity.IsAuthenticated) return null;

            return identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        private async Task<Customer> GetCurrentCustomer()
        {
            try
            {
                var accountId = GetAuthenticatedAccountId();
                if (string.IsNullOrEmpty(accountId))
                    return null;

                // Lấy customerId từ accountId
                var customerId = await _registerAttendRepo.GetCustomerIdByAccountId(accountId);
                if (string.IsNullOrEmpty(customerId))
                    return null;

                // Lấy thông tin customer từ repository
                var customer = await _customerRepo.GetCustomerById(customerId);
                if (customer == null)
                    return null;

                // Load thêm thông tin Account nếu cần
                if (customer.Account == null)
                {
                    customer = await _customerRepo.GetCustomerByAccountId(customerId);
                }

                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin customer hiện tại");
                return null;
            }
        }

        private string GenerateShortGuid()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 16);
        }

        private int ConvertDateTimeToUnixTimestamp(DateTime date)
        {
            var dateTimeOffset = new DateTimeOffset(date);
            return (int)dateTimeOffset.ToUnixTimeSeconds();
        }

        private async Task CheckPendingPayments(string customerId, PaymentTypeEnums serviceType)
        {
            var pendingOrders = await _orderRepo.GetOrdersByCustomerAndService(customerId, serviceType);
            var hasPendingPayment = pendingOrders.Any(o => o.Status == PaymentStatusEnums.Pending.ToString());
            
            if (hasPendingPayment)
            {
                throw new AppException(
                    ResponseCodeConstants.BAD_REQUEST, 
                    $"Bạn có đơn hàng {serviceType.ToString()} chưa thanh toán. Vui lòng thanh toán trước khi đặt dịch vụ mới.", 
                    StatusCodes.Status400BadRequest
                );
            }
        }
    }
}
