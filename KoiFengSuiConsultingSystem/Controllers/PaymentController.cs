using System;
using System.Threading.Tasks;
using BusinessObjects;
using BusinessObjects.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.ApiModels.Payment;
using Services.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Net.payOS.Types;
using Services.Services.PaymentService;
using BusinessObjects.Exceptions;
using Services.ServicesHelpers.RefundSerivce;
using Services.Services.OrderService;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IRefundService _refundService;
        private readonly IOrderService _orderService;
        public PaymentController(IPaymentService paymentService, IRefundService refundService, IOrderService orderService)
        {
            _paymentService = paymentService;
            _refundService = refundService;
            _orderService = orderService;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("payos/customer/payment-url")]
        public async Task<IActionResult> GetPayOSPaymentUrl(PaymentTypeEnums paymentType, string serviceId)
        {
            var res = await _paymentService.Payment(serviceId, paymentType);
            return Ok(res);
        }

        [HttpPost("refund")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ProcessRefund([FromBody] RefundRequest request)
        {
            try
            {
                var customerQR = await _refundService.ProcessRefundAsync(request);
                return Ok(new { CustomerRefundQR = customerQR });
            }
            catch (AppException ex)
            {
                return StatusCode(ex.StatusCode, new { ex.Code, ex.Message });
            }
        }

        [HttpGet("get-manager-refunded")]
        public async Task<IActionResult> GetManagerRefunded()
        {
            var res = await _orderService.GetManagerRefunded();
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-manager-refunded-for-mobile")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetManagerRefundedForCustomer()
        {
            var res = await _orderService.GetManagerRefundedForCustomer();
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("manager-confirm-refunded")]
        public async Task<IActionResult> ManagerConfirmRefunded(string id)
        {
            var res = await _orderService.ManagerConfirmRefunded(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("customer-confirm-received")]
        public async Task<IActionResult> CustomerConfirmReceived(string id)
        {
            var res = await _orderService.CustomerConfirmReceived(id);
            return StatusCode(res.StatusCode, res);
        }
    }
}