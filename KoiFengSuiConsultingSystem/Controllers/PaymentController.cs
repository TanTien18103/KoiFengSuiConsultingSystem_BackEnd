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

        [HttpPost("service/{serviceType}/{serviceId}")]
        //[Authorize]
        public async Task<ActionResult<PaymentResponse>> CreateServicePayment(string serviceType, string serviceId)
        {
            try
            {
                // Chuyển đổi serviceType từ string sang enum PaymentTypeEnums
                if (!Enum.TryParse<PaymentTypeEnums>(serviceType, true, out var serviceTypeEnum))
                {
                    return BadRequest(new { message = "Loại dịch vụ không hợp lệ" });
                }
                
                // Gọi phương thức duy nhất để xử lý thanh toán
                var response = await _paymentService.CreatePaymentForService(serviceTypeEnum, serviceId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("create")]
        //[Authorize]
        public async Task<ActionResult<PaymentResponse>> CreatePayment([FromBody] PaymentRequest request)
        {
            try
            {
                // Tự động điền thông tin khách hàng
                request = await _paymentService.PopulateCustomerInfoForPaymentRequest(request);
                
                // Tạo thanh toán
                var response = await _paymentService.CreatePaymentAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("status/{orderId}")]
        //[Authorize]
        public async Task<ActionResult<PaymentResponse>> CheckPaymentStatus(string orderId)
        {
            try
            {
                var response = await _paymentService.CheckPaymentStatusAsync(orderId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
} 