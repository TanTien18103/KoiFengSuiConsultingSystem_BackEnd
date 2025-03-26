using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.Contract;
using Services.Services.ContractService;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;

        public ContractController(IContractService contractService)
        {
            _contractService = contractService;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateContract([FromForm] ContractRequest request)
        {
            var result = await _contractService.CreateContract(request);
            return StatusCode(result.StatusCode, result);
        }
        //[Authorize(Roles = "Customer, Manager")]
        [HttpPatch("cancel/{contractId}")]
        public async Task<IActionResult> CancelContract(string contractId)
        {
            var result = await _contractService.CancelContract(contractId);
            return StatusCode(result.StatusCode, result);
        }
        //[Authorize(Roles = "Customer, Manager")]
        [HttpPatch("confirm/{contractId}")]
        public async Task<IActionResult> ConfirmContract(string contractId)
        {
            var result = await _contractService.ConfirmContract(contractId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get-contract-by-booking-offline-and-update-status/{bookingOfflineId}")]
        public async Task<IActionResult> GetContractByBookingOfflineIdAndUpdateStatus(string bookingOfflineId)
        {
            var result = await _contractService.GetContractByBookingOfflineIdAndUpdateStatus(bookingOfflineId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("send-otp/{contractId}")]
        public async Task<IActionResult> SendOtpForContract(string contractId)
        {
            var result = await _contractService.SendOtpForContract(contractId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("verify-otp/{contractId}")]
        public async Task<IActionResult> VerifyContractOtp(string contractId, [FromForm] VerifyOtpRequest request)
        {
            var result = await _contractService.VerifyContractOtp(contractId, request);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("{contractId}/first-payment")]
        public async Task<IActionResult> ProcessFirstPayment(string contractId)
        {
            var result = await _contractService.ProcessFirstPaymentAfterVerification(contractId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
