using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.Attachment;
using Services.Services.AttachmentService;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentService _attachmentService;

        public AttachmentController(IAttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
        }
        [HttpPost]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> CreateAttachment([FromForm] AttachmentRequest request)
        {
            var result = await _attachmentService.CreateAttachment(request);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("{attachmentId}")]
        [Authorize]
        public async Task<IActionResult> GetAttachmentById(string attachmentId)
        {
            var result = await _attachmentService.GetAttachmentById(attachmentId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("booking/{bookingOfflineId}")]
        [Authorize]
        public async Task<IActionResult> GetAttachmentByBookingOfflineId(string bookingOfflineId)
        {
            var result = await _attachmentService.GetAttachmentByBookingOfflineId(bookingOfflineId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPut("cancel/{attachmentId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CancelAttachment(string attachmentId)
        {
            var result = await _attachmentService.CancelAttachment(attachmentId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("confirm/{attachmentId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> ConfirmAttachment(string attachmentId)
        {
            var result = await _attachmentService.ConfirmAttachment(attachmentId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("send-otp/{attachmentId}")]
        [Authorize]
        public async Task<IActionResult> SendOtpForAttachment(string attachmentId)
        {
            var result = await _attachmentService.SendOtpForAttachment(attachmentId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("verify-otp/{attachmentId}")]
        [Authorize]
        public async Task<IActionResult> VerifyAttachmentOtp(string attachmentId, [FromBody] VerifyOtpRequest request)
        {
            var result = await _attachmentService.VerifyAttachmentOtp(attachmentId, request);
            return StatusCode(result.StatusCode, result);
        }
    }
}
