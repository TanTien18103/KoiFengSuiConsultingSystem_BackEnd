﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.Attachment;
﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        [HttpPost("create")]
        [Authorize(Roles = "Master")]
        public async Task<IActionResult> CreateAttachment([FromForm] AttachmentRequest request)
        {
            var result = await _attachmentService.CreateAttachment(request);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("{attachmentId}")]
        //[Authorize]
        public async Task<IActionResult> GetAttachmentById(string attachmentId)
        {
            var result = await _attachmentService.GetAttachmentById(attachmentId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllAttachments()
        {
            var result = await _attachmentService.GetAllAttachments();
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("booking/{bookingOfflineId}")]
        [Authorize]
        public async Task<IActionResult> GetAttachmentByBookingOfflineId(string bookingOfflineId)
        {
            var result = await _attachmentService.GetAttachmentByBookingOfflineId(bookingOfflineId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPatch("cancel/{attachmentId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CancelAttachment(string attachmentId)
        {
            var result = await _attachmentService.CancelAttachment(attachmentId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPatch("confirm/{attachmentId}")]
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
        [HttpGet("get-all-by-master")]
        [Authorize(Roles = "Master")]
        public async Task<IActionResult> GetAllAttachmentsByMaster()
        {
            var result = await _attachmentService.GetAllAttachmentsByMaster();
            return StatusCode(result.StatusCode, result);
        }
    }
}
