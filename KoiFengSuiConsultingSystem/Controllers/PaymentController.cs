using System;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.ApiModels.Payment;
using Services.Interfaces;
using Services.Services;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<ActionResult<PaymentResponse>> CreatePayment([FromBody] PaymentRequest request)
        {
            try
            {
                // Tạo OrderId duy nhất
                request.OrderId = $"{request.PaymentType}_{request.ServiceId}_{DateTime.Now.Ticks}";
                
                // Thiết lập URL callback
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                request.ReturnUrl = $"{baseUrl}/api/payment/success";
                request.CancelUrl = $"{baseUrl}/api/payment/cancel";

                var response = await _paymentService.CreatePaymentAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("status/{orderId}")]
        [Authorize]
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

        [HttpGet("success")]
        public async Task<IActionResult> PaymentSuccess([FromQuery] string orderId)
        {
            try
            {
                var paymentStatus = await _paymentService.CheckPaymentStatusAsync(orderId);
                
                if (paymentStatus.Status == "SUCCESS")
                {
                    // Xử lý cập nhật trạng thái đơn hàng tại đây
                    // Có thể thêm logic để cập nhật trạng thái của Booking/Course/Workshop
                    
                    return Redirect("/payment-success"); // Redirect to success page
                }
                
                return Redirect("/payment-failed");
            }
            catch (Exception)
            {
                return Redirect("/payment-failed");
            }
        }

        [HttpGet("cancel")]
        public IActionResult PaymentCancel([FromQuery] string orderId)
        {
            // Xử lý khi người dùng hủy thanh toán
            return Redirect("/payment-cancelled");
        }
    }
} 