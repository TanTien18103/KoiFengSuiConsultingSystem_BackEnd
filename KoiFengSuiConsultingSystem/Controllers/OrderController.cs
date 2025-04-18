using BusinessObjects.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Services.OrderService;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("get-pending-order")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetPendingOrders()
        {
            var res = await _orderService.GetPendingOrders();
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("update-to-PAID/{id}")]
        //[Authorize(Roles = "Master")]
        public async Task<IActionResult> UpdateToPaid([FromRoute] string id)
        {
            var res = await _orderService.UpdateOrderToPaid(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("update-to-PENDINGCONFIRM/{id}")]
        public async Task<IActionResult> UpdateToPendingConfirm([FromRoute] string id)
        {
            var res = await _orderService.UpdateOrderToPendingConfirm(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("cancel/{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CancelOrder(string id, PaymentTypeEnums type)
        {
            var res = await _orderService.CancelOrder(id, type);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-order/{id}")]
        public async Task<IActionResult> GetDetailsOrder([FromRoute]string id)
        {
            var res = await _orderService.GetDetailsOrder(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-pendingConfirm-order")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetPendingConfirmOrders()
        {
            var res = await _orderService.GetPendingConfirmOrders();
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-waitingForRefund-order")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetWaitingForRefundOrders()
        {
            var res = await _orderService.GetWaitingForRefundOrders();
            return StatusCode(res.StatusCode, res);
        }
    }
}
