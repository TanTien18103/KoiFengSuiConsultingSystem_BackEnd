using Microsoft.AspNetCore.Http;
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAttachmentById([FromRoute] string id)
        {
            var result = await _attachmentService.GetAttachmentById(id);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllAttachments()
        {
            var result = await _attachmentService.GetAllAttachments();
            return StatusCode(result.StatusCode, result);
        }
    }
}
