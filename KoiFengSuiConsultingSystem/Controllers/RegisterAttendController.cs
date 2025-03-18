using BusinessObjects.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.RegisterAttend;
using Services.Services.RegisterAttendService;

namespace KoiFengSuiConsultingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterAttendController : ControllerBase
    {
        private readonly IRegisterAttendService _registerAttendService;
        public RegisterAttendController(IRegisterAttendService registerAttendService)
        {
            _registerAttendService = registerAttendService;
        }

        [HttpGet("get-by-status")]
        public async Task<IActionResult> GetRegisterAttends([FromQuery] RegisterAttendStatusEnums? status = null)
        {
            var res = await _registerAttendService.GetRegisterAttends(status);
            return StatusCode(res.StatusCode, res);
        }
      
        [HttpGet("customer")]
        public async Task<IActionResult> GetRegisterAttendByCustomerId()
        {
            var result = await _registerAttendService.GetRegisterAttendByCustomerId();
            return StatusCode(result.StatusCode, result);
        }
        
        [HttpGet("{registerAttendId}")]
        public async Task<IActionResult> GetRegisterAttendById([FromRoute] string registerAttendId)
        {
            var result = await _registerAttendService.GetRegisterAttendById(registerAttendId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetRegisterAttends()
        {
            var result = await _registerAttendService.GetRegisterAttends();
            return StatusCode(result.StatusCode, result);
        }

      
        [HttpGet("workshop/{id}")]
        public async Task<IActionResult> GetRegisterAttendByWorkshopId([FromRoute] string id)
        {
            var result = await _registerAttendService.GetRegisterAttendByWorkshopId(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("register-by-group/{id}")]
        public async Task<IActionResult> GetRegisterAttendByGroupId([FromRoute] string id)
        {
            var result = await _registerAttendService.GetRegisterAttendByGroupId(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRegisterAttend([FromBody] RegisterAttendRequest request)
        {
            var result = await _registerAttendService.CreateRegisterAttend(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRegisterAttend(string id, int num)
        {
            var result = await _registerAttendService.UpdatePendingTickets(id, num);
            return StatusCode(result.StatusCode, result);
        }
    }
}
