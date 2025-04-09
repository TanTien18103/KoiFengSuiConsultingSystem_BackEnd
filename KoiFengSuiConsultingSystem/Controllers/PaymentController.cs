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

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IRefundService _refundService;
        public PaymentController(IPaymentService paymentService, IRefundService refundService)
        {
            _paymentService = paymentService;
            _refundService = refundService;
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
        //[HttpPost("payos/transfer-handler")]
        //public IActionResult PayOSPaymentExecute([FromBody] WebhookType request)
        //{
        //    _paymentService.GetWebhookTypeAsync(request);
        //    return Ok();
        //}

        //[Authorize(Roles = "Admin")]
        //[HttpGet("payos/admin/payment-info/{orderCode}")]
        //public async Task<IActionResult> GetPayOSPaymentInfo([FromRoute] long orderCode)
        //{
        //    var response = await _paymentService.GetPaymentLinkInformationAsync(orderCode);
        //    return Ok(response);
        //}

        //[Authorize(Roles = "Customer")]
        //[HttpGet("payos/customer/confirmation/{orderId}/{orderCode}")]
        //public async Task<IActionResult> PayOSConfirmPayment([FromRoute] string orderId, long orderCode)
        //{
        //    await _paymentService.ConfirmPayment(orderId, orderCode);
        //    return Ok();
        //}
    }
}