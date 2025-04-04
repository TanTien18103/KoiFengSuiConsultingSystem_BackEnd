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
        [HttpGet("by-bookingOffline/{id}")]
        public async Task<IActionResult> GetContractByBookingOfflineId([FromRoute] string id)
        {
            var result = await _contractService.GetContractByBookingOfflineId(id);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateContract([FromForm] ContractRequest request)
        {
            var result = await _contractService.CreateContract(request);
            return StatusCode(result.StatusCode, result);
        }
        [Authorize(Roles = "Manager")]
        [HttpPatch("manager/cancel/{contractId}")]
        public async Task<IActionResult> CancelContractByMaster(string contractId)
        {
            var result = await _contractService.CancelContractByMaster(contractId);
            return StatusCode(result.StatusCode, result);
        }
        [Authorize(Roles = "Manager")]
        [HttpPatch("manager/confirm/{contractId}")]
        public async Task<IActionResult> ConfirmContractByMaster(string contractId)
        {
            var result = await _contractService.ConfirmContractByMaster(contractId);
            return StatusCode(result.StatusCode, result);
        }
        [Authorize(Roles = "Customer")]
        [HttpPatch("customer/cancel/{contractId}")]
        public async Task<IActionResult> CancelContractByCustomer(string contractId)
        {
            var result = await _contractService.CancelContractByCustomer(contractId);
            return StatusCode(result.StatusCode, result);
        }
        [Authorize(Roles = "Customer, Manager")]
        [HttpPatch("customer/confirm/{contractId}")]
        public async Task<IActionResult> ConfirmContractByCustomer(string contractId)
        {
            var result = await _contractService.ConfirmContractByCustomer(contractId);
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContractById([FromRoute] string id)
        {
            var result = await _contractService.GetContractById(id);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllContracts()
        {
            var result = await _contractService.GetAllContracts();
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get-all-by-staff")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> GetAllContractByStaff()
        {
            var result = await _contractService.GetAllContractByStaff();
            return StatusCode(result.StatusCode, result);
        }
    }
}
