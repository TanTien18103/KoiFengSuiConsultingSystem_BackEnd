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
using Net.payOS.Types;

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

        [Authorize(Roles = "Customer")]
        [HttpPost("payos/customer/payment-url")]
        public async Task<IActionResult> GetPayOSPaymentUrl([FromBody] PayOSRequest request)
        {
            var response = await _paymentService.CreatePaymentLinkAsync(request);
            return Ok(response);
        }

        [HttpPost("payos/transfer-handler")]
        public IActionResult PayOSPaymentExecute([FromBody] WebhookType request)
        {
            _paymentService.GetWebhookTypeAsync(request);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("payos/admin/payment-info/{orderCode}")]
        public async Task<IActionResult> GetPayOSPaymentInfo([FromRoute] long orderCode)
        {
            var response = await _paymentService.GetPaymentLinkInformationAsync(orderCode);
            return Ok(response);
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("payos/customer/confirmation/{orderId}/{orderCode}")]
        public async Task<IActionResult> PayOSConfirmPayment([FromRoute] string orderId, long orderCode)
        {
            await _paymentService.ConfirmPayment(orderId, orderCode);
            return Ok();
        }
    }
}