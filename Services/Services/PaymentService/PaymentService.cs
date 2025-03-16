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
            IPayOSService payOSService)
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
        }
        public static string GenerateShortGuid()
        {
            Guid guid = Guid.NewGuid();
            string base64 = Convert.ToBase64String(guid.ToByteArray());
            return base64.Replace("/", "_").Replace("+", "-").Substring(0, 20);
        }
        public async Task<CreatePaymentResult> CreateServicePaymentLinkAsync(PaymentTypeEnums serviceType, string serviceId, string cancelUrl, string returnUrl)
        {
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                throw new AppException(ResponseCodeConstants.UNAUTHORIZED, ResponseMessageIdentity.TOKEN_NOT_SEND, StatusCodes.Status401Unauthorized);
            }
            var token = authHeader.Substring("Bearer ".Length);
            if (string.IsNullOrEmpty(token))
            {
                throw new AppException(ResponseCodeConstants.UNAUTHORIZED, ResponseMessageIdentity.TOKEN_INVALID, StatusCodes.Status401Unauthorized);
            }
            var curAccount = await _accountRepo.GetAccountIdFromToken(token);
            if (string.IsNullOrEmpty(curAccount))
            {
                throw new AppException(ResponseCodeConstants.UNAUTHORIZED, ResponseMessageIdentity.TOKEN_INVALID_OR_EXPIRED, StatusCodes.Status401Unauthorized);
            }

            var curCustomer = await _customerRepo.GetCustomerByAccountId(curAccount);
            if (curCustomer == null)
            {
                throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstantsUser.CUSTOMER_NOT_FOUND, StatusCodes.Status404NotFound);
            }
            // Lấy thông tin giá từ PriceService
            var price = await _priceService.GetServicePrice(serviceType, serviceId);
            if (price == null || price <= 0)
            {
                throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstrantsOrder.PRICE_NOT_FOUND_OR_INVALID, StatusCodes.Status404NotFound);
            }

            // Tạo mô tả dịch vụ dựa trên loại dịch vụ
            string description = "";
            string customerName = "";

            switch (serviceType)
            {
                case PaymentTypeEnums.BookingOnline:
                    var bookingOnline = await _bookingOnlineRepo.GetBookingOnlineByIdRepo(serviceId);
                    if (bookingOnline == null)
                        throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstrantsBooking.NOT_FOUND_ONLINE, StatusCodes.Status404NotFound);

                    description = $"Thanh toán đặt lịch trực tuyến";
                    customerName = curCustomer.Account.FullName ?? "Khách hàng";
                    break;

                case PaymentTypeEnums.BookingOffline:
                    var bookingOffline = await _bookingOfflineRepo.GetBookingOfflineById(serviceId);
                    if (bookingOffline == null)
                        throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstrantsBooking.NOT_FOUND_OFFLINE, StatusCodes.Status404NotFound);

                    description = $"Gói tư vấn: {bookingOffline.ConsultationPackage?.PackageName}";
                    customerName = curCustomer.Account.FullName ?? "Khách hàng";
                    break;

                case PaymentTypeEnums.Course:
                    var course = await _courseRepo.GetCourseById(serviceId);
                    if (course == null)
                        throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstrantsCourse.COURSE_NOT_FOUND, StatusCodes.Status404NotFound);

                    description = $"Khóa học: {course.CourseName}";
                    // Vì không có thông tin về khách hàng trực tiếp trong bảng Course
                    customerName = curCustomer.Account.FullName ?? "Khách hàng";
                    break;

                case PaymentTypeEnums.Workshop:
                    var workshop = await _workShopRepo.GetWorkShopById(serviceId);
                    if (workshop == null)
                        throw new AppException(ResponseCodeConstants.NOT_FOUND, ResponseMessageConstrantsWorkshop.WORKSHOP_NOT_FOUND, StatusCodes.Status404NotFound);

                    description = $"Sự kiện: {workshop.WorkshopName}";
                    // Vì không có thông tin về khách hàng trực tiếp trong bảng Workshop
                    customerName = curCustomer.Account.FullName ?? "Khách hàng";
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
                $"{description}",
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
            return createPayment;
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

        private int ConvertDateTimeToUnixTimestamp(DateTimeOffset dateTime)
        {
            return (int)dateTime.ToUnixTimeSeconds();
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
    }
}
