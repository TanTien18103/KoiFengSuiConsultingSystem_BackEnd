using System;
using System.Threading.Tasks;
using BusinessObjects;
using BusinessObjects.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.ApiModels.Payment;
using Services.Interfaces;
using Services.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ICustomerService _customerService;
        private readonly IAccountService _accountService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IBookingService _bookingOnlineService;

        public PaymentController(
            IPaymentService paymentService,
            ICustomerService customerService,
            IAccountService accountService,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            IBookingService bookingOnlineService)
        {
            _paymentService = paymentService;
            _customerService = customerService;
            _accountService = accountService;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _bookingOnlineService = bookingOnlineService;
        }

        /// <summary>
        /// Tạo link thanh toán cho dịch vụ
        /// </summary>
        [HttpPost("create-payment-link")]
        [Authorize]
        public async Task<IActionResult> CreatePaymentLink(
            [FromQuery] PaymentTypeEnums serviceType,
            [FromQuery] string serviceId,
            [FromQuery] decimal amount,
            [FromQuery] string returnUrl = null,
            [FromQuery] string cancelUrl = null)
        {
            try
            {
                var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(customerId))
                {
                    return Unauthorized("Không tìm thấy thông tin người dùng.");
                }

                // Kiểm tra các trường bắt buộc
                if (string.IsNullOrEmpty(serviceId) || amount <= 0)
                {
                    return BadRequest("ServiceId và Amount là bắt buộc và Amount phải lớn hơn 0.");
                }

                // Tạo URL cho trang thành công và hủy
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                string fullReturnUrl = string.IsNullOrEmpty(returnUrl) ?
                    $"{baseUrl}/payment/success" :
                    (returnUrl.StartsWith("http") ? returnUrl : $"{baseUrl}/{returnUrl.TrimStart('/')}");

                string fullCancelUrl = string.IsNullOrEmpty(cancelUrl) ?
                    $"{baseUrl}/payment/cancel" :
                    (cancelUrl.StartsWith("http") ? cancelUrl : $"{baseUrl}/{cancelUrl.TrimStart('/')}");

                var paymentLink = await _paymentService.CreatePaymentLinkAsync(
                    serviceType,
                    serviceId,
                    amount,
                    customerId,
                    fullReturnUrl,
                    fullCancelUrl);

                return Ok(new
                {
                    success = true,
                    data = paymentLink
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Lỗi khi tạo link thanh toán: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Webhook nhận thông báo kết quả thanh toán từ PayOS
        /// </summary>
        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> Webhook([FromBody] WebhookRequest request)
        {
            try
            {
                var result = await _paymentService.ProcessWebhookAsync(request);

                if (result)
                {
                    return Ok(new { success = true });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Không thể xử lý webhook" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Lỗi khi xử lý webhook: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Đăng ký URL webhook với PayOS
        /// </summary>
        [HttpPost("register-webhook")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterWebhook()
        {
            try
            {
                var webhookUrl = await _paymentService.RegisterWebhookUrl();

                return Ok(new
                {
                    success = true,
                    webhookUrl = webhookUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Lỗi khi đăng ký webhook: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Kiểm tra trạng thái thanh toán của một đơn hàng
        /// </summary>
        [HttpGet("check-payment-status/{orderCode}")]
        [Authorize]
        public async Task<IActionResult> CheckPaymentStatus(string orderCode)
        {
            try
            {
                // Giả sử bạn có phương thức kiểm tra trạng thái thanh toán trong service
                // var status = await _paymentService.CheckPaymentStatusAsync(orderCode);

                // Tạm thời trả về NotImplemented
                return StatusCode(501, new
                {
                    success = false,
                    message = "Chức năng này chưa được triển khai"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Lỗi khi kiểm tra trạng thái thanh toán: {ex.Message}"
                });
            }
        }
    }
} 